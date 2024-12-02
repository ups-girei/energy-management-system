using Girei.Grid.DataCollector.DataProducer.Data.Repositories;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Girei.Grid.DataCollector.DataProducer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRedisRepository _redisRepository;
        private readonly IConfiguration _configuration;
        private IConnection _rabbitMqConnection;
        private IModel _rabbitMqChannel;
        private string _hostName;
        private string _queueName;
        private int _take;

        private List<string> _deviceTypes;
        

        private void InitializeRabbitMQ()
        {
            _hostName = _configuration.GetValue<string>("RabbitMQ:HostName");// Use "localhost" if RabbitMQ is running locally or via Docker
            _queueName = _configuration.GetValue<string>("RabbitMQ:Queue");

            var factory = new ConnectionFactory()
            {
                HostName = _hostName 
            };

            _rabbitMqConnection = factory.CreateConnection();
            _rabbitMqChannel = _rabbitMqConnection.CreateModel();

            _rabbitMqChannel.QueueDeclare(queue: _queueName,
                                          durable: false,
                                          exclusive: false,
                                          autoDelete: false,
                                          arguments: null);

        }
        public Worker(ILogger<Worker> logger, IConfiguration configuration, IRedisRepository redisRepository)
        {
            _logger = logger;
            _redisRepository = redisRepository;
            _configuration = configuration;
            _take = _configuration.GetValue<int>("RedisSettings:Take", defaultValue: 10);
            _deviceTypes = configuration.GetSection("RedisSettings:DeviceTypes").Get<List<string>>();
            InitializeRabbitMQ();
        }
        public async Task PushDeviceDataAsync(string pattern)
        {
            try
            {
                // Get the latest device data using the generic repository method
                var deviceDataList = await _redisRepository.GetLatestDeviceDataAsync(pattern, _take);
                foreach (var data in deviceDataList)
                {
                    // Convert the dynamic data into a dictionary
                    var dataDictionary = new Dictionary<string, object>();

                    // Enumerate the properties if data is a JsonElement
                    if (data is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var field in jsonElement.EnumerateObject())
                        {
                            dataDictionary.Add(field.Name, field.Value);
                        }
                    }

                    var message = JsonSerializer.Serialize(dataDictionary);

                    var body = Encoding.UTF8.GetBytes(message);

                    _rabbitMqChannel.BasicPublish(exchange: "",routingKey: _queueName,basicProperties: null,body: body);
                    
                    _logger.LogInformation($"[x] Sent {pattern} data for device {dataDictionary["DeviceId"]?.ToString()}");

                    await _redisRepository.DeleteDeviceDataByValueAsync($"{pattern.Split(':')[0]}:{dataDictionary["DeviceId"]?.ToString()}", data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing {pattern} data.");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var deviceType in _deviceTypes)
                {
                    await PushDeviceDataAsync($"{deviceType}:*");
                }
                var delayMilliseconds = _configuration.GetValue<int>("RabbitMQ:PollingIntervalMilliseconds");
                await Task.Delay(delayMilliseconds, stoppingToken);
            }
        }
        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker is stopping.");

            // Clean up RabbitMQ channel and connection
            _rabbitMqChannel?.Close();
            _rabbitMqChannel?.Dispose();
            _rabbitMqConnection?.Close();
            _rabbitMqConnection?.Dispose();

            return base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _rabbitMqChannel?.Dispose();
            _rabbitMqConnection?.Dispose();
            base.Dispose();
        }
    }
}
