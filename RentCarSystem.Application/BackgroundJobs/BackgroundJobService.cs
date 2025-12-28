using Hangfire;
using RentCarSystem.Application.Common.Interfaces;
using Serilog;

namespace RentCarSystem.Application.BackgroundJobs
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IEmailService _emailService;

        public BackgroundJobService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void EnqueuePasswordResetEmail(string email, string token)
        {
            BackgroundJob.Enqueue(() => SendPasswordResetEmailAsync(email, token));
            Log.Information(" Password reset email job enqueued for: {Email}", email);
        }

        public void EnqueueVerificationEmail(string email, string token)
        {
            BackgroundJob.Enqueue(() => SendVerificationEmailAsync(email, token));
            Log.Information("Verification email job enqueued for: {Email}", email);
        }

        public void EnqueueWelcomeEmail(string email, string userName)
        {
            BackgroundJob.Enqueue(() => SendWelcomeEmailAsync(email, userName));
            Log.Information("Welcome email job enqueued for: {Email}", email);
        }

        // Hangfire çalıştıracak metodlar (public olmalı!)
        public async Task SendPasswordResetEmailAsync(string email, string token)
        {
            await _emailService.SendPasswordResetEmailAsync(email, token);
        }

        public async Task SendVerificationEmailAsync(string email, string token)
        {
            await _emailService.SendVerificationEmailAsync(email, token);
        }

        public async Task SendWelcomeEmailAsync(string email, string userName)
        {
            await _emailService.SendWelcomeEmailAsync(email, userName);
        }
    }
}