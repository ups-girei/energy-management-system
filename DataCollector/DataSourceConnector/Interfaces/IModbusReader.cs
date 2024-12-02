
namespace Girei.Grid.DataCollector.DataSourceConnector.Interfaces
{
    public interface IModbusReader
    {
        Task<List<dynamic>> ReadAllDevicesAsync();
    }
}
