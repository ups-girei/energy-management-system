using StackExchange.Redis;

namespace Girei.Grid.DataCollector.DataProducer.Data.Repositories
{
    public interface IRedisRepository
    {
        IEnumerable<RedisKey> GetRedisKeys(string pattern);
        Task<List<dynamic>> GetLatestDeviceDataAsync(string pattern, int count = 1);
        Task DeleteDeviceDataByValueAsync(string key, dynamic deviceData);
    }
}
