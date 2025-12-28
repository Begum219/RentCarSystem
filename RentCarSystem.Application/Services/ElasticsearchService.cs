using Nest;

namespace RentCarSystem.Application.Services
{
    public interface IElasticsearchService
    {
        Task IndexVehicleAsync(int vehicleId, string data);
        Task<dynamic> SearchVehiclesAsync(string query);  
        Task DeleteIndexAsync(string indexName);
    }

    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _client;
        private const string IndexName = "vehicles";

        public ElasticsearchService()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex(IndexName);
            _client = new ElasticClient(settings);
        }

        public async Task IndexVehicleAsync(int vehicleId, string data)
        {
            var response = await _client.IndexAsync(
                new { data },
                i => i
                    .Index(IndexName)
                    .Id(vehicleId.ToString())
            );

            if (!response.IsValid)
                throw new Exception($"Elasticsearch error: {response.ServerError?.Error?.Reason}");
        }

        public async Task<dynamic> SearchVehiclesAsync(string query)
        {
            var response = await _client.SearchAsync<dynamic>(s => s
                .Index(IndexName)
                .Query(q => q
                    .MultiMatch(m => m
                        .Query(query)
                        .Fields(f => f.Field("*"))
                    )
                )
            );

            // Sadece documentsi döndür response al 
            if (response.IsValid && response.Hits.Count > 0)
            {
                return new
                {
                    success = true,
                    total = response.Total,
                    hits = response.Hits.Select(h => h.Source).ToList()
                };
            }

            return new
            {
                success = response.IsValid,
                total = response.Total,
                hits = new List<dynamic>(),
                message = response.IsValid ? "No results found" : "Search failed",
                error = response.ServerError?.Error?.Reason
            };
        }

        public async Task DeleteIndexAsync(string indexName)
        {
            await _client.Indices.DeleteAsync(indexName);
        }
    }
}