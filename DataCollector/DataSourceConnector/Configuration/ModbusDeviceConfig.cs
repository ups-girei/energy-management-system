namespace Girei.Grid.DataCollector.DataSourceConnector.Configuration
{
    public class Register
    {
        public ushort StartAddress { get; set; }
        public List<Value> Values { get; set; }
    }

    public class Value
    {
        public string Description { get; set; }
    }

    public class Device
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public List<Register> Registers { get; set; }
    }
    
    public class ModbusDeviceConfig
    {
        public byte SlaveAddress { get; set; }
        public List<Device> Devices { get; set; }
    }


    
}


