
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ScanModbus;
using Serilog;
using System.IO;


class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .UseSerilog((context, loggerConfig) =>
            {
                loggerConfig.ReadFrom.Configuration(context.Configuration);
            })
            .Build();

        var configuration = host.Services.GetRequiredService<IConfiguration>();

        // Read configuration
        var ipAddress = configuration["ModbusSettings:IPAddress"];
        var port = int.Parse(configuration["ModbusSettings:Port"]);
        var startAddress = ushort.Parse(configuration["ModbusSettings:StartAddress"]);
        var endAddress = ushort.Parse(configuration["ModbusSettings:EndAddress"]);


        var pathResults = configuration["ModbusSettings:Results"]?? "ScanResults";

        if (!Directory.Exists(pathResults))
        {
            Directory.CreateDirectory(pathResults);
        }



        var availableRegisters = new List<ushort>();
        var unavailableRegisters = new List<ushort>();

        Log.Information("Starting register scan...");

        try
        {
            // Update ModbusReaderService with configuration values
            ModbusReaderService modbusReaderService = new ModbusReaderService(ipAddress, port);

            for (ushort address = startAddress; address <= endAddress; address++)
            {
                try
                {
                    ushort[] values = await modbusReaderService.ReadRegistersAsync(address, 1);
                    Log.Information($"Register {address} is available. Value: {values[0]}");
                    availableRegisters.Add(address);
                }
                catch
                {
                    Log.Warning($"Register {address} is not available.");
                    unavailableRegisters.Add(address);
                }
            }

            // Save results to files
            await File.WriteAllLinesAsync(Path.Combine(pathResults, "AvailableRegisters.txt"), availableRegisters.ConvertAll(a => a.ToString()));
            await File.WriteAllLinesAsync(Path.Combine(pathResults, "UnavailableRegisters.txt"), unavailableRegisters.ConvertAll(a => a.ToString()));

            Log.Information("Scan completed. Results saved to files.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred during the scan.");
        }
    }
}

