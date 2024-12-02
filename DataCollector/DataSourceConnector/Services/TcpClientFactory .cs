using Girei.Grid.DataCollector.DataSourceConnector.Interfaces;
using System.Net.Sockets;

namespace Girei.Grid.DataCollector.DataSourceConnector.Services
{
    public class TcpClientFactory : ITcpClientFactory
    {
        public TcpClient CreateClient(string ipAddress, int port)
        {
            return new TcpClient(ipAddress, port);
        }
    }
}
