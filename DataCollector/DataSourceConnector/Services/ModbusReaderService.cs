using Girei.Grid.DataCollector.DataSourceConnector.Configuration;
using Girei.Grid.DataCollector.DataSourceConnector.DeviceReaders;
using Girei.Grid.DataCollector.DataSourceConnector.Interfaces;
using NModbus;
using NModbus.Extensions.Enron;
using System.Net.Sockets;
using static StackExchange.Redis.Role;

namespace Girei.Grid.DataCollector.DataSourceConnector.Services
{
    public class ModbusReaderService : IModbusReader
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private readonly ModbusDeviceConfig _deviceSettings;
        private readonly ITcpClientFactory _tcpClientFactory;


        public ModbusReaderService(string ipAddress, int port, IConfiguration configuration, ITcpClientFactory tcpClientFactory)
        {
            _ipAddress = ipAddress;
            _port = port;
            _deviceSettings = configuration.GetSection("ModbusDevices").Get<ModbusDeviceConfig>(); // Load device configs
            _tcpClientFactory = tcpClientFactory;
        }

        private async Task<ushort[]> ReadRegistersAsync(ushort startAddress, int numRegisters)
        {
            using (TcpClient client = new TcpClient(_ipAddress, _port))
            {
                var factory = new ModbusFactory();
                var master = factory.CreateMaster(client);
                return await Task.Run(() => master.ReadHoldingRegisters(_deviceSettings.SlaveAddress, startAddress, (ushort)numRegisters));

            }
        }
        public async Task<List<dynamic>> ReadAllDevicesAsync()
        {
            var deviceDataList = new List<dynamic>();

            foreach (var config in _deviceSettings.Devices)
            {
                DeviceReader deviceReader;
                deviceReader = new DeviceReader(config);

                List<ushort> regitersConsolidate = new List<ushort>();
                foreach (var register in config.Registers)
                {
                    var registers = await ReadRegistersAsync(register.StartAddress, register.Values.Count);
                    regitersConsolidate.AddRange(registers);
                }
                deviceDataList.Add(await deviceReader.ReadDataAsync(regitersConsolidate.ToArray()));
            }
            return deviceDataList;
        }
    }
}
