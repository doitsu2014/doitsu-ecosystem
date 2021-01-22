using System;
using System.IO;
using System.Reflection;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Shared.Abstraction;
using static LanguageExt.Prelude;

namespace Shared.ConsoleApp.Extension
{
    public static class ServiceProviderFactory
    {
        private static IConfigurationBuilder _configurationBuilder;

        private static IConfiguration GetConfiguration(string jsonFilePath = null, Assembly assembly = null)
        {
            if (_configurationBuilder == null)
            {
                _configurationBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(jsonFilePath.IsNullOrWhitespace() ? "Configurations/appSettings.json" : jsonFilePath, false)
                    .AddUserSecrets(assembly == null ? Assembly.GetAssembly(typeof(ServiceProviderFactory)) : assembly);
            }

            return _configurationBuilder.Build();
        }

        public static Option<IServiceProvider> GetServiceProvider(Action<ServiceCollection, IConfiguration> addCustomServices, string jsonFilePath = null, Assembly assembly = null)
        {
            return Some((jsonFilePath, assembly))
                .Map(req => GetConfiguration(req.jsonFilePath, req.assembly))
                .Map<IServiceProvider>(conf =>
                {
                    var sc = new ServiceCollection();
                    addCustomServices(sc, conf);
                    return sc.AddSingleton(conf)
                        .AddLogging(l => l.AddSerilog(new LoggerConfiguration().ReadFrom.Configuration(conf).CreateLogger()))
                        .BuildServiceProvider();
                });
        }
    }
}