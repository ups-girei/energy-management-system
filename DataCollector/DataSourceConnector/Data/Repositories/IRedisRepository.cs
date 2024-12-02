namespace Girei.Grid.DataCollector.DataSourceConnector.Data.Repositories
{
    public interface IRedisRepository
    {
        Task SaveDeviceDataAsync(dynamic deviceData);
    }
}
