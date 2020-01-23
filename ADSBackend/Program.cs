using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using Scholarships.Models.ConfigurationViewModels;

namespace Scholarships
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Get the environment
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Temporarily use the appsettings.json file to get the log directory
            var bconfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json")
                .Build();


            // Now load the paths object and bind to a new configPath object
            var configPath = new ConfigPath();
            bconfig.GetSection("Paths").Bind(configPath);

            // Set up the logger now that we have the log path
            string logPath = configPath.LogPath;
            Directory.CreateDirectory(logPath);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, 
                                        flushToDiskInterval: TimeSpan.FromSeconds(1),
                                        shared: true)
                .CreateLogger();


            try
            {
                Log.Information("Starting web host");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {

            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()) // the path where the json file should be loaded from
                                                          .AddEnvironmentVariables()
                                                          .Build();


            return WebHost.CreateDefaultBuilder(args)
                          .UseConfiguration(configuration)
                          .UseStartup<Startup>()
                          .UseSerilog()
                          .Build();
        }
    }
}
