using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Utility.Extensions;
using Utility.Interfaces;
using Utility.Services;
using Utility.Settings;

namespace Utility.Kafka.ConsoleApp
{
    public static class ServiceProviderFactory
    {
        private static IConfigurationBuilder _configurationBuilder;

        private static IConfiguration GetConfiguration(string[] args)
        {
            if (_configurationBuilder == null)
            {
                _configurationBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Configurations/appSettings.json", false);
                _configurationBuilder.AddUserSecrets(Assembly.GetAssembly(typeof(Program)), true);
            }
            return _configurationBuilder.Build();
        }

        public static IServiceProvider GetServiceProvider(string[] args)
        {
            return new ServiceCollection()
                .Map(services =>
                {
                    var configuration = GetConfiguration(args);
                    services.AddSingleton(configuration);
                    services.AddTransient<IKafkaService, KafkaService>();
                    services.Configure<KafkaSettings>(configuration.GetSection("KafkaSettings"));
                    return services;
                })
                .Map(s =>
                {
                    var logger = new LoggerConfiguration().ReadFrom.Configuration(GetConfiguration(args)).CreateLogger();
                    return s.AddLogging(l => l.ClearProviders().AddSerilog(logger));
                })
                .BuildServiceProvider();
        }
    }
}