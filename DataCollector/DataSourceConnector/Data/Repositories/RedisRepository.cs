using StackExchange.Redis;
using System.Text.Json;

namespace Girei.Grid.DataCollector.DataSourceConnector.Data.Repositories
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase _database;
        readonly int _expirationInMinutes = 60;

        public RedisRepository(IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
        {
            _database = connectionMultiplexer.GetDatabase();
            _expirationInMinutes = configuration.GetValue<int>("RedisSettings:ExpirationInMinutes");
        }

        private async Task EnsureSortedSetAsync(string key)
        {
            var keyType = await _database.KeyTypeAsync(key);
            if (keyType != RedisType.SortedSet && keyType != RedisType.None)
            {
                // If the key exists and is not a sorted set, delete it
                await _database.KeyDeleteAsync(key);
                // Consider using a logger instead of Console.WriteLine
                Console.WriteLine($"Key {key} deleted because it was of type {keyType}");
            }
        }

        public async Task SaveDeviceDataAsync(dynamic deviceData)
        {
            if (deviceData == null)
            {
                return;
            }
            
            var key = $"{deviceData.DeviceType}:{deviceData.DeviceId}";
            var jsonData = JsonSerializer.Serialize(deviceData);
            var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            await EnsureSortedSetAsync(key);
            await _database.SortedSetAddAsync(key, jsonData, timestamp);

            TimeSpan expiration = TimeSpan.FromMinutes(_expirationInMinutes);
            await _database.KeyExpireAsync(key, expiration);

            Console.WriteLine($"{deviceData.DeviceType} data saved successfully.");
        }
    }
}
