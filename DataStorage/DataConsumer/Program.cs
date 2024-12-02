using Girei.Grid.DataStorage.DataConsumer;
using Serilog;
using Serilog.Sinks.Elasticsearch;
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
                IndexFormat = $"{indexFormat}{DateTime.UtcNow:yyyy-MM}",
                NumberOfShards = 2,
                NumberOfReplicas = 1
            })
            .Enrich.WithProperty("Application", Assembly.GetExecutingAssembly().GetName().Name)
            .ReadFrom.Configuration(context.Configuration);
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
