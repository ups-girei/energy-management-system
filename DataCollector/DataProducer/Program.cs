using Girei.Grid.DataCollector.DataProducer;
using Girei.Grid.DataCollector.DataProducer.Data.Repositories;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using StackExchange.Redis;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, services, configuration) =>
    {
        var elasticUri = context.Configuration.GetValue<string>("ElasticSearchSettings:Uri");
        var indexFormat = context.Configuration.GetValue<string>("ElasticSearchSettings:IndexFormat");
        configuration
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{indexFormat}{DateTime.UtcNow:yyyy-MM}", // Creates monthly indexes
                NumberOfShards = 2,
                NumberOfReplicas = 1
            })
            .Enrich.WithProperty("Application", Assembly.GetExecutingAssembly().GetName().Name)
            .ReadFrom.Configuration(context.Configuration);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var ipAddress = configuration.GetValue<string>("RedisSettings:IpAddress");
            var port = configuration.GetValue<int>("RedisSettings:Port");

            return ConnectionMultiplexer.Connect($"{ipAddress}:{port}");
        });

        services.AddSingleton<IRedisRepository, RedisRepository>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
