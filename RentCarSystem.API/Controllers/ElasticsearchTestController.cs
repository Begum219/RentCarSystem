using Iyzipay.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using RentCarSystem.Application.Services;
using static System.Net.Mime.MediaTypeNames;

namespace RentCarSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ElasticsearchTestController : ControllerBase
    {
        private readonly IElasticsearchService _elasticsearchService;

        public ElasticsearchTestController(IElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }

        [HttpPost("index")]
        public async Task<IActionResult> IndexData([FromBody] dynamic data)
        {
            await _elasticsearchService.IndexVehicleAsync(1, System.Text.Json.JsonSerializer.Serialize(data));
            return Ok("Indexed successfully");
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var result = await _elasticsearchService.SearchVehiclesAsync(query);
            return Ok(result);
        }
    }
}
