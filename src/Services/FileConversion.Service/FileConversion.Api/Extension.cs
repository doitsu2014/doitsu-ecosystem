using ACOMSaaS.NetCore.Abstractions.Model;
using ACOMSaaS.NetCore.EFCore.Abstractions.Interface;
using FileConversion.Abstraction;
using FileConversion.Data;
using FileConversion.Core.Interface;
using FileConversion.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Reflection;

namespace FileConversion.Api
{
    public static class Extension
    {
        public static void ConfigureGlobalExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError("Unhandled exception:\n{Exception}", contextFeature.Error);

                        var serializerSettings = new JsonSerializerSettings();
                        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        var content = JsonConvert.SerializeObject(new SimpleMessageResponse
                        {
                            Value = contextFeature.Error.Message
                        }, serializerSettings);
                        await context.Response.WriteAsync(content);
                    }
                });
            });
        }

        public static void ConfigureFileConversionContext(WebHostBuilderContext context, IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var migrationAssembly = typeof(FileConversionContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddEntityFrameworkNpgsql().AddDbContext<FileConversionContext>((prod, builder) =>
            {
                builder.UseApplicationServiceProvider(serviceProvider);
                builder.UseNpgsql(
                    context.Configuration.GetConnectionString(Constants.ConnectionStringKey),
                    sql => sql.MigrationsAssembly(migrationAssembly));
                builder.UseLoggerFactory(loggerFactory);
            });
        }
    }
}
