
using Girei.Grid.DataCollector.DataSourceConnector.Configuration;
using System.Dynamic;

namespace Girei.Grid.DataCollector.DataSourceConnector.DeviceReaders
{
    public class DeviceReader
    {
        Device _deviceConfig;

        public DeviceReader(Device deviceConfig) 
        { 
            _deviceConfig = deviceConfig; 
        }

        public Task<dynamic> ReadDataAsync(ushort[] registers)
        {
            dynamic deviceObject = new ExpandoObject();
            var deviceDict = (IDictionary<string, object>)deviceObject;
            deviceDict["DeviceId"] = _deviceConfig.DeviceId;
            deviceDict["DeviceType"] = _deviceConfig.DeviceType;
            int index = 0;
            foreach (var register in _deviceConfig.Registers)
            {
                foreach (var value in register.Values)
                {
                    deviceDict[value.Description] = registers[index];
                    index++;
                }
            }
            deviceDict["Timestamp"] = DateTime.UtcNow;
            return Task.FromResult<dynamic>(deviceObject);
        }
    }
}
