
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Girei.Grid.DataStorage.DataConsumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        #region RabbitMQ
        private IModel _channel;
        private IConnection _connection;
        private string _hostName;
        private string _queueName;
        #endregion

        #region InfluxDB
        private InfluxDBClient _influxDbClient;
        private string _bucket;
        private string _org;
        #endregion

        private void InitializeRabbitMQ()
        {
            _hostName = _configuration.GetValue<string>("RabbitMQ:HostName");// Use "localhost" if RabbitMQ is running locally or via Docker
            _queueName = _configuration.GetValue<string>("RabbitMQ:Queue");

            var factory = new ConnectionFactory()
            {
                HostName = _hostName // Use "localhost" if RabbitMQ is running locally or via Docker
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare the queue to make sure it exists
            _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }
        private void InitializeInfluxDB()
        {
            // Retrieve InfluxDB settings from configuration
            var influxUri = _configuration.GetValue<string>("InfluxDBSettings:Uri");
            var token = _configuration.GetValue<string>("InfluxDBSettings:Token");
            _bucket = _configuration.GetValue<string>("InfluxDBSettings:Bucket");
            _org = _configuration.GetValue<string>("InfluxDBSettings:Org");

            // Initialize the InfluxDB client
            _influxDbClient = InfluxDBClientFactory.Create(influxUri, token.ToCharArray());
        }

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            InitializeRabbitMQ();
            InitializeInfluxDB();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Log the received message
                _logger.LogInformation($"[x] Received message: {message}");
                
                // Process the message (e.g., save it to a database or other storage)
                ProcessMessage(message);
            };

            _channel.BasicConsume(queue: _queueName,
                                  autoAck: true,
                                  consumer: consumer);

            return Task.CompletedTask;

        }

        private void ProcessMessage(string message)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var deviceObject = JsonSerializer.Deserialize<Dictionary<string, object>>(message, options);
                WriteDataToInfluxDB(deviceObject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message and saving to InfluxDB");
            }
        }

        private async void WriteDataToInfluxDB(Dictionary<string, object> deviceData)
        {

            string measurementType = deviceData.ContainsKey("DeviceType") ? deviceData["DeviceType"]?.ToString() : "Unknown";

            var tags = new Dictionary<string, string>
                {
                    { "DeviceId", deviceData["DeviceId"]?.ToString() }
                };

            var fields = deviceData.Where(kvp => kvp.Key != "DeviceId" && kvp.Key != "DeviceType").ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var point = PointData.Measurement(measurementType).Tag("DeviceId", tags["DeviceId"]);
            if (deviceData.TryGetValue("Timestamp", out var timestamp) && DateTime.TryParse(timestamp?.ToString(), out var parsedTimestamp))
            {
                point = point.Timestamp(parsedTimestamp, WritePrecision.Ns);
            }

            foreach (var field in fields)
            {
                if (field.Value is JsonElement jsonElement)
                {
                    if (jsonElement.ValueKind == JsonValueKind.Number && jsonElement.TryGetDouble(out double numericValue))
                    {
                        point = point.Field(field.Key, numericValue);
                    }
                    else
                    {
                        Console.WriteLine($"Field '{field.Key}' is not a number or cannot be parsed.");
                    }
                }
            }
            
            try
            {
                using (var writeApi = _influxDbClient.GetWriteApi())
                {
                    await Task.Run(() => writeApi.WritePoint(point, _bucket, _org));
                    _logger.LogInformation($"Data point for {deviceData["DeviceId"]?.ToString()} written to InfluxDB.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing data to InfluxDB.");
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker is stopping.");

            // Close RabbitMQ channel and connection gracefully
            _channel?.Close();
            _connection?.Close();

            return base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
