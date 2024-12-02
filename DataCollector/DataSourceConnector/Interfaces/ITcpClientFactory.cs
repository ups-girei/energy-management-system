using System.Net.Sockets;

namespace Girei.Grid.DataCollector.DataSourceConnector.Interfaces
{
    public interface ITcpClientFactory
    {
        TcpClient CreateClient(string ipAddress, int port);
    }
}
