# Data Consumer
## _Overview_

Data Consumer is a background service designed to consume messages from RabbitMQ and store them in a InfluxDB.
## _Key Features_
- **RabbitMQ Integration:**: Consumes messages from a RabbitMQ queue.
- **InfluxDB Support**: Stores processed or raw data into an InfluxDB bucket.
- **Flexible Configuration:**: Easily adjustable through the appsettings.json file.
- **Background Service**: Can run seamlessly as a background service or in a containerized environment like Docker.

## _Prerequisites_
## Software Requirements
- **.NET 6.0**
- **RabbitMQ** (e.g., Docker RabbitMQ instance or local installation)
- **InfluxDB**
- **Elasticsearch**
## Dependencies
Install the following NuGet packages:
- **Serilog** (for logging)
- **StackExchange.Redis** (for Redis integration)
- **RabbitMQ.Client** (for RabbitMQ support)
- **InfluxDbB.Client** (for interacting with InfluxDB)

## _Configurations_
## Key Sections
- **Serilog**: Logging levels for monitoring and debugging.
- **ElasticSearchSettings**: Configures logging integration with Elasticsearch.
- **RabbitMQ**: Connection settings for consuming messages.
- **InfluxDBSettings**: Connection and authentication details for writing to InfluxDB.

## _How to Run_
## Local Environment
- Ensure **InfluxDB**, **RabbitMQ** and **Elasticsearch** are installed and running.
    - InfluxDB default UI: :  http://localhost:8086
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
    - On Windows:`.\Girei.Grid.DataStorage.DataConsumer.exe`
    - On Windows:`./Girei.Grid.DataStorage.DataConsumer`
## Docker
- Build the Docker image:
`docker build -t venus-gx-data-consumer .`
- Run the container:
`docker run -d --name data-consumer --network=host venus-gx-data-consumer`

## _Usage_
- The service continuously listens to the specified RabbitMQ queue (data_queue) for incoming messages.
- Messages are processed, optionally formatted or calculated, and then stored in the designated InfluxDB bucket.
