using StackExchange.Redis;
using System.Text.Json;

namespace Girei.Grid.DataCollector.DataProducer.Data.Repositories
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisRepository(ILogger<RedisRepository> logger, IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
        {
            _redis = connectionMultiplexer;
            _database = connectionMultiplexer.GetDatabase();
        }

        public IEnumerable<RedisKey> GetRedisKeys(string pattern)
        {
            var result = new List<RedisKey>();
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            result.AddRange(server.Keys(pattern: pattern));
            return result;
        }

        public async Task<List<dynamic>> GetLatestDeviceDataAsync(string pattern, int count = 1)
        {
            var allDeviceData = new List<dynamic>();
            var keys = GetRedisKeys(pattern);

            foreach (var key in keys)
            {
                // Get the most recent entries for each device
                var redisValues = await _database.SortedSetRangeByScoreAsync(key, order: Order.Descending, take: count);

                if (redisValues.Length > 0)
                {
                    allDeviceData.AddRange(redisValues.Select(value => JsonSerializer.Deserialize<dynamic>(value)).ToList());
                }
            }
            return allDeviceData;
        }

        public async Task DeleteDeviceDataByValueAsync(string key, dynamic deviceData)
        {
            // Serialize the device data back to JSON format as stored in Redis
            var jsonData = JsonSerializer.Serialize(deviceData);

            // Remove the entry from the sorted set based on the value
            await _database.SortedSetRemoveAsync(key, jsonData);

        }
    }
}
