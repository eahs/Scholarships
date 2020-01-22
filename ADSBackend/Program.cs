using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace Scholarships
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            // Temporarily use the appsettings.json file to get the log directory
            var bconfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

#if DEBUG
            var section = bconfig.GetSection("FilePaths:Development");
#else
            var section = bconfig.GetSection("FilePaths:Production");
#endif
            string lpath = section["LogPath"] ?? Path.Combine("App_Data", "Logs");

            string logPath = Path.Combine(lpath, "");
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
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            return WebHost.CreateDefaultBuilder(args)
                          .UseConfiguration(configuration)
                          .UseStartup<Startup>()
                          .UseSerilog()
                          .Build();
        }
    }
}
