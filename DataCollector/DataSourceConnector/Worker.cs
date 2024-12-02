using Girei.Grid.DataCollector.DataSourceConnector.Data.Repositories;
using Girei.Grid.DataCollector.DataSourceConnector.Interfaces;

namespace Girei.Grid.DataCollector.DataSourceConnector
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly IModbusReader _modbusReader; // Inject the Modbus reader
        private readonly IRedisRepository _redisRepository; // Inject Redis repository
        private readonly IConfiguration _configuration; // Inject IConfiguration

        public Worker(ILogger<Worker> logger, IModbusReader modbusReader, IRedisRepository redisRepository, IConfiguration configuration)
        {
            _logger = logger;
            _modbusReader = modbusReader;
            _redisRepository = redisRepository;
            _configuration = configuration;
        }

        private async Task ReadAndSaveDataAsync(CancellationToken stoppingToken)
        {
            var deviceData = await _modbusReader.ReadAllDevicesAsync();
            foreach (var data in deviceData)
            {
                await _redisRepository.SaveDeviceDataAsync(data);
            }
            _logger.LogInformation("Data read and saved at: {time}", DateTimeOffset.Now);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await ReadAndSaveDataAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while reading or saving data");
                }

                // Read the delay value from configuration
                var delayMilliseconds = _configuration.GetValue<int>("ModbusSettings:PollingIntervalInMiliseconds");
                await Task.Delay(delayMilliseconds, stoppingToken); // Use configured delay
            }
        }
    }
}
