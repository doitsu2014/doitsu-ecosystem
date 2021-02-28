using System;
using System.Threading.Tasks;
using Blazor.FileConversion.Client.HttpClients.FileConversion;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blazor.FileConversion.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient(FileConversionHttpClient.HttpClientKey,
                client =>
                {
                    client.BaseAddress =
                        new Uri(builder.Configuration[
                            $"{FileConversionHttpClient.FileConversionHttpClientSettings}:BaseAddress"]);
                }).AddHttpMessageHandler<FileConversionAuthorizationMessageHandler>();

            builder.Services.TryAddScoped<FileConversionAuthorizationMessageHandler>();
            builder.Services.AddTransient<IFileConversionHttpClient, FileConversionHttpClient>();
            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Oidc", options.ProviderOptions);
            });

            await builder.Build().RunAsync();
        }
    }
}