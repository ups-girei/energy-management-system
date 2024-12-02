# Data Producer
## _Overview_

The DataProducer is a background service designed to extract data from Redis, process it, and push it into RabbitMQ. 
This ensures efficient utilization of Redis resources while making data available to external systems for further processing or analysis.

## _Key Features_
- **Redis Integration**: Reads data from a Redis database, ensuring timely consumption and freeing up resources.
- **RabbitMQ Support**: Publishes data to RabbitMQ queues for integration with external systems..
- **Flexible Configuration**: Customizable settings for Redis, RabbitMQ, and logging in appsettings.json.
- **Scalable and Reliable**: Designed as a background service, suitable for deployment in containerized environments.

## _Prerequisites_
## Software Requirements
- **.NET 6.0**
- **Redis Database** (e.g., Docker Redis instance or local installation)
- **RabbitMQ** (e.g., Docker RabbitMQ instance or local installation)
- **Elasticsearch**
## Dependencies
Install the following NuGet packages:
- **Serilog** (for logging)
- **StackExchange.Redis** (for Redis integration)
- **RabbitMQ.Client** (for RabbitMQ support)

## _Configurations_
## Key Sections
- **Serilog**: Configures logging levels.
- **RedisSetting**s: Defines Redis connection details, the number of items to process (Take), and specific device types.
- **ElasticSearchSettings**: Sets up Elasticsearch endpoint and log index naming.
- **RabbitMQ**: Specifies the RabbitMQ host, target queue, and polling interval.

## _How to Run_
## Local Environment
- Ensure **Redis**, **RabbitMQ** and **Elasticsearch** are installed and running.
    - Default Redis port: 6379
    - Default RabbitMQ management UI:  http://localhost:15672
    - Default Elasticsearch URI: http://localhost:9200
- Clone the repository and restore dependencies:
`dotnet restore`
- Clone the repository and restore dependencies:
`dotnet build`
`dotnet run`

## Creating an Executable
- Publish the application to create a self-contained executable:
`dotnet publish -c Release -r win-x64 --self-contained true -o ./publish`
Replace win-x64 with the runtime identifier (rid) for your target platform (e.g., linux-x64, osx-x64).
- Navigate to the ./publish directory:
`cd publish`
- Run the executable
    - On Windows:`.\Girei.Grid.DataCollector.DataProducer.exe`
    - On Windows:`./Girei.Grid.DataCollector.DataProducer`
## Docker
- Build the Docker image:
`docker build -t venus-gx-data-producer .`
- Run the container:
`docker run -d --name data-producer --network=host venus-gx-data-producer`

## _Usage_
The application continuously polls data from Redis and publishes it to the specified RabbitMQ queue. Configuration settings can be updated in appsettings.json for:

- Adjusting polling intervals
- Adding/removing device types
- Changing the target RabbitMQ queue