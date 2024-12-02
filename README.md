# Proof of Concept for GIREI Web Application Architecture
## _Overview_

This project serves as a proof of concept for the architecture designed for the final master's project titled “Diseño de la arquitectura de una aplicación web para el almacenamiento y publicación de parámetros eléctricos del GIREI.” The goal of the project is to capture and store electrical data provided by the Venus GX device into an InfluxDB database.
The proof of concept focuses on the development of two core components:

- **Data Collector Service**: Collects data from the Venus GX.
- **Data Storage Service**: Stores and processes this data in InfluxDB.
These components work together to validate the architecture and demonstrate its functionality for the final application.

## _Key Features_
- **Modular Components**: The architecture is broken down into clear components such as Data Collector Service and Data Storage Service.
- **Flexible Data Flow**: Data from Venus GX devices is collected, processed, and stored for later use and publication.
- **Configurable Settings**: Various components like data collection intervals, device addresses, and storage settings can be configured through the appsettings.json file.
## Software Requirements
The projects are developed in .NET 6.0 and requires the installation of several dependencies:
- **Serilog**: For logging and monitoring the application.
- **StackExchange.Redis**: For interacting with Redis (optional for some services).
- **InfluxDB.Client**: For storing electrical parameter data into InfluxDB.
- **NModbus**: For handling Modbus communication with Venus GX.
- **RabbitMQ.Client** (for RabbitMQ support)

Also, make sure to:
- **Venus GX Device**: Configured to provide electrical data via Modbus.
- Read the **README** files for each project.

## Contributing
Contributions are welcome! Please fork the repository, create a branch for your feature or bugfix, and submit a pull request. When contributing, make sure to follow the coding guidelines and include relevant tests.
