using StackExchange.Redis;
using System.Text.Json;

namespace DummyProject.Service
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _redis;

        public CacheService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _redis.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _redis.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(10));
        }
    }
}
