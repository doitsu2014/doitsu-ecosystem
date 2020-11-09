// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Identity.Service.IdentityServer;
using Identity.Service.IdentityServer.Data;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace Doitsu.Ecosystem.Identity.Service
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
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = CreateHostBuilder(args)
                    .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                    .Build();

                Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                host.MigrateDbContext<PersistedGrantDbContext>((_, __) => { })
                    .MigrateDbContext<ApplicationDbContext>((context, services) =>
                    {
                        var env = services.GetService<IWebHostEnvironment>();
                        var logger = services.GetService<ILogger<ApplicationDbContextSeed>>();
                        var settings = services.GetService<IOptions<AppSettings>>();

                        new ApplicationDbContextSeed()
                            .SeedAsync(context, env, logger, settings)
                            .Wait();
                    })
                    .MigrateDbContext<ConfigurationDbContext>((context, services) =>
                    {
                        new ConfigurationDbContextSeed()
                            .SeedAsync(context, configuration)
                            .Wait();
                    });

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();

                return 0;


                // var seed = args.Contains("/seed");
                // if (seed)
                // {
                //     args = args.Except(new[] { "/seed" }).ToArray();
                // }

                // var host = CreateHostBuilder(args).Build();

                // if (seed)
                // {
                //     Log.Information("Seeding database...");
                //     var config = host.Services.GetRequiredService<IConfiguration>();
                //     SeedData.EnsureSeedData(config.GetConnectionString(ApplicationConstants.CS_CONFIGURATION_DB), config.GetConnectionString(ApplicationConstants.CS_PERSISTEDGRANT_DB));
                //     Log.Information("Done seeding database.");
                //     return 0;
                // }

                // Log.Information("Starting host...");
                // host.Run();
                // return 0;
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
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            var config = builder.Build();
            return builder.Build();
        }
    }
}