# Data Source Connector
## _Overview_

The Data Source Connector is a background service designed to obtain data from a Venus GX device using the Modbus protocol. The collected data is stored temporarily in a Redis repository and logged in Elasticsearch for monitoring and analysis.
## _Key Features_
- **Modbus Integration**: Polls data from Venus GX devices using Modbus TCP.
- **Flexible Configuration**: All configurations, including Redis, Elasticsearch, and Modbus settings, are customizable via appsettings.json.
- **Scalable Data Storage**: Temporarily stores data in Redis with configurable expiration times.
- **Background Service**: Can run seamlessly as a background service or in a containerized environment like Docker.
## _Prerequisites_
## Software Requirements
- **.NET 6.0**
- **Redis Database** (e.g., Docker Redis instance or local installation)
- **Elasticsearch**
## Dependencies
Install the following NuGet packages:
- **Serilog** (for logging)
- **StackExchange.Redis** (for Redis integration)
- **NModbus** (for Modbus protocol support)

## _Configurations_
## Key Sections
- **Serilog**: Configures logging levels.
- **RedisSetting**s: Defines Redis connection settings and expiration times.
- **ElasticSearchSettings**: Sets up Elasticsearch endpoint and log index naming.
- **ModbusSettings**: Specifies the Modbus server's IP address, port, and polling interval.
- **ModbusDevices**: Lists devices and registers to poll, along with their descriptions.

## _How to Run_
## Local Environment
- Ensure **Redis** and **Elasticsearch** are installed and running.
    - Default Redis port: 6379
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
    - On Windows:`.\Girei.Grid.DataCollector.DataSourceConnector.exe`
    - On Windows:`./Girei.Grid.DataCollector.DataSourceConnector`
## Docker
- Build the Docker image:
`docker build -t venus-gx-data-source-connector .`
- Run the container:
`docker run -d --name data-source-connector --network=host venus-gx-data-source-connector`

## _Usage_
The application runs as a background service, polling data from Venus GX devices at regular intervals. Data is temporarily stored in Redis and logged into Elasticsearch for further analysis and visualization.

To change configuration settings, update the appsettings.json file and restart the application.