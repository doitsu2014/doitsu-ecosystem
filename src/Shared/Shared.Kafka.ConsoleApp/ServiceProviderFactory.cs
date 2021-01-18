using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstraction.Interfaces.Services;
using Shared.Abstraction.Settings;
using Shared.Services;
using static LanguageExt.Prelude;

namespace Shared.Kafka.ConsoleApp
{
    public static class ServiceProviderFactory1
    {
        // private static IConfigurationBuilder _configurationBuilder;
        //
        // private static IConfiguration GetConfiguration(string[] args)
        // {
        //     if (_configurationBuilder == null)
        //     {
        //         _configurationBuilder = new ConfigurationBuilder()
        //             .SetBasePath(Directory.GetCurrentDirectory())
        //             .AddJsonFile("Configurations/appSettings.json", false);
        //         _configurationBuilder.AddUserSecrets(Assembly.GetAssembly(typeof(Program)), true);
        //     }
        //     return _configurationBuilder.Build();
        // }
        //
        // public static IServiceProvider GetServiceProvider2()
        // {
        //     Shared.ConsoleApp.Extension.ServiceProviderFactory.
        //     return Some((sc: new ServiceCollection(), conf: GetConfiguration(args)))
        //         .Map(req =>
        //         {
        //             var configuration = req.conf;
        //             req.sc.AddSingleton(configuration);
        //             req.sc.AddTransient<IKafkaService, KafkaService>();
        //             req.sc.Configure<KafkaSettings>(configuration.GetSection("KafkaSettings"));
        //             return req.sc;
        //         })
        //         .Map(s =>
        //         {
        //             var logger = new LoggerConfiguration().ReadFrom.Configuration(GetConfiguration(args)).CreateLogger();
        //             return s.AddLogging(l => l.ClearProviders().AddSerilog(logger));
        //         })
        //         .BuildServiceProvider();
        // }
    }
}