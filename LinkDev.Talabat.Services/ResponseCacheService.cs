using LinkDev.Talabat.Core.Services.Contracts;
using StackExchange.Redis;
using System.Text.Json;

namespace LinkDev.Talabat.Application
{
    public class ResponseCacheService(IConnectionMultiplexer connectionMultiplexer) : IResponseCacheService
    {
        private readonly IDatabase _database = connectionMultiplexer.GetDatabase();

        public async Task CacheResponseAsync(string key, object response, TimeSpan timeToLive)
        {
            if (response is null) return;
            var serializationOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var serializedResponse = JsonSerializer.Serialize(response, serializationOptions);
            await _database.StringSetAsync(key, serializedResponse, timeToLive);
        }

        public async Task<string?> GetCachedResponseAsync(string key)
        {
            var response = await _database.StringGetAsync(key);
            if (response.IsNullOrEmpty) return null;
            return response;
        }
    }
}
