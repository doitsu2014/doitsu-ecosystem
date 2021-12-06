using System;
using System.Globalization;
using System.IO;
using System.Text;
using Amazon.S3;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;
using RollingInterval = Serilog.Sinks.AmazonS3.RollingInterval;

namespace Identity.Service.OpenIdServer
{
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);
            try
            {
                Log.Information("Starting Webhost...", AppName);
                CreateHostBuilder(args)
                    .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                    .UseSerilog()
                    .Build()
                    .Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var builder = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);
            // builder.WriteTo.MinIO(setting => configuration.GetSection("SerilogMinIOSetting").Bind(setting));
            return builder
                .CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Startup>()
                .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}