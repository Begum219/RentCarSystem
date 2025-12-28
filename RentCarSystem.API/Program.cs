using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RentCarSystem.Application.Common.Models;
using RentCarSystem.Infrastructure;
using RentCarSystem.Application;
using RentCarSystem.API.Middleware;
using System.Text;
using Serilog;
using Hangfire;
using Hangfire.SqlServer;
using RentCarSystem.Application.Orchestrators;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using RentCarSystem.Domain.Entities;
using AspNetCoreRateLimit;
using Hangfire.Logging;
using RentCarSystem.Application.Services;
using Elastic.Apm.NetCoreAll;

//  Serilog yapılandırması (En başta)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting RentCarSystem API...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog'u Host'a ekle
    builder.Host.UseSerilog();

    // JWT Settings
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

    // Email settings
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

    // Iyzico Settings
    builder.Services.Configure<IyzicoSettings>(builder.Configuration.GetSection("IyzicoSettings"));

    // Services
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplicationServices();

    // Orchestrator
    builder.Services.AddScoped<ReservationOrchestrator>();

    // Redis Cache
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = "localhost:6379";
        options.InstanceName = "RentCarSystem:";
    });

    // Rate limiting servisleri
    builder.Services.AddMemoryCache();
    builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
    builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
    builder.Services.AddInMemoryRateLimiting();
    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

    // Hangfire
    builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

    builder.Services.AddHangfireServer();

    // JWT Authentication 
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    // OData Model Builder
    var modelBuilder = new ODataConventionModelBuilder();
    modelBuilder.EntitySet<Vehicle>("Vehicles");
    modelBuilder.EntitySet<Reservation>("Reservations");
    modelBuilder.EntitySet<User>("Users");
    modelBuilder.EntitySet<Brand>("Brands");
    modelBuilder.EntitySet<Category>("Categories");

    // Controllers with OData
    builder.Services.AddControllers()
        .AddOData(options => options
            .Select()
            .Filter()
            .OrderBy()
            .Expand()
            .Count()
            .SetMaxTop(100)
            .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // Swagger JWT İLE
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "RentCar System API",
            Version = "v1",
            Description = "Araç Kiralama Sistemi API - JWT Authentication with Serilog + OData"
        });

        // JWT için Swagger yapılandırması 
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer eyJhbGci...'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // ✅ APM Services (Parametresiz)
    builder.Services.AddAllElasticApm();
    builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();

    var app = builder.Build();

    // ✅ APM MIDDLEWARE - MUTLAKA EN ÜSTTE, EXCEPTION MIDDLEWARE'DEN ÖNCE
    app.UseAllElasticApm(builder.Configuration);

    // Middleware Pipeline
    app.UseMiddleware<GlobalExceptionMiddleware>();

    // IP DEBUG LOG
    app.Use(async (context, next) =>
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString();
        Log.Information("🔍 Client IP: {ClientIp}", clientIp);
        await next();
    });

    app.UseIpRateLimiting();  //rate limiting middleware

    app.UseSerilogRequestLogging();

    // Hangfire Dashboard
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() }
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("RentCarSystem API started successfully on {Environment}", app.Environment.EnvironmentName);
    Log.Information("Swagger: https://localhost:7026/swagger");
    Log.Information("Hangfire Dashboard: https://localhost:7026/hangfire");
    Log.Information("OData Metadata: https://localhost:7026/odata/$metadata");
    Log.Information("⚡ Rate Limiting: Enabled");
    Log.Information("📊 Elastic APM: http://localhost:8200");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Shutting down RentCarSystem API...");
    Log.CloseAndFlush();
}