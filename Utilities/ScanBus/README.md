# Scanbus
## _Overview_

Scanbus is a console application that scans a Modbus network to check which addresses are available and which are not. The scan is configurable via the appsettings.json file.

## _Key Features_
- **Modbus Address Scanning**: Scans specified Modbus address ranges to check availability.
- **Configurable**: Modbus settings such as IP address, port, and address range are configurable through the appsettings.json file.
- **Logging**: Logs scanning results to the console and to a log file with configurable logging levels and rolling intervals using Serilog.


## _Prerequisites_
## Software Requirements
- **.NET 6.0**
- **Serilog** (for logging)
- **NModbus ** (for Modbus communication)
- **Elasticsearch**
## Dependencies
Install the following NuGet packages:
- **Serilog**
- **NModbus**

## _Configurations_
## Key Sections
- **ModbusSettings**
    - **IPAddress**: IP address of the Modbus server.
    - **Port**: Port number for the Modbus server (default: 502).
    - **StartAddress and EndAddress**: Defines the range of Modbus addresses to scan.
    - **ResultPaths**: Path where scan results are saved.
- **Serilog**
    - Configures logging to both the console and a rolling log file.
    - You can adjust the logging level and other Serilog settings here.

## _How to Run_
## Local Environment
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
    - On Windows:`.\Scanbus.exe`
    - On Windows:`./Scanbus`
## Docker
- Build the Docker image:
`docker build -t venus-gx-data-consumer .`
- Run the container:
`docker run -d --name data-consumer --network=host venus-gx-data-consumer`

## _Usage_
- After the application is running, it will begin scanning the specified Modbus address range (StartAddress to EndAddress) and log the results.
- The results of the scan will be saved in the directory defined by the ResultPaths setting.
- Logs will be written to the console and the logs/log.txt file, with rolling intervals of one day.