using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;

namespace Blazor.FileConversion.Client.HttpClients.FileConversion
{
    public class FileConversionAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public FileConversionAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation,
            IConfiguration configuration) :
            base(provider, navigation)
        {
            ConfigureHandler(
                authorizedUrls: new[]
                    {configuration[$"{FileConversionHttpClient.FileConversionHttpClientSettings}:BaseAddress"]},
                scopes: new[] {"services.fileconversion.parse"});
        }
    }
}