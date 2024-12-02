

using NModbus;
using System.Net.Sockets;

namespace ScanModbus
{
    public class ModbusReaderService
    {
        private readonly string _ipAddress;
        private readonly int _port;


        public ModbusReaderService(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public async Task<ushort[]> ReadRegistersAsync(ushort startAddress, int numRegisters)
        {
            using (TcpClient client = new TcpClient(_ipAddress, _port))
            {
                var factory = new ModbusFactory();
                var master = factory.CreateMaster(client);
                return await Task.Run(() => master.ReadHoldingRegisters(100, startAddress, (ushort)numRegisters));

            }
        }
    }
}
