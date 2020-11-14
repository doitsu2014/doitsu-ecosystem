using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Identity.Service.OpenIdServer.Constants;
using Identity.Service.OpenIdServer.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Service.OpenIdServer
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken);

            await RegisterApplicationsAsync(scope.ServiceProvider);
            await RegisterScopesAsync(scope.ServiceProvider);

            static async Task RegisterApplicationsAsync(IServiceProvider provider)
            {
                var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                var applicationSection = configuration.GetSection("ApplicationManager");

                if (await manager.FindByClientIdAsync("blogpost-blazor-client") is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "blogpost-blazor-client",
                        ConsentType = ConsentTypes.Explicit,
                        DisplayName = "Blazor client application",
                        Type = ClientTypes.Public,
                        PostLogoutRedirectUris =
                        {
                            new Uri($"{applicationSection["BlazorClient:Uri"]}/authentication/logout-callback")
                        },
                        RedirectUris =
                        {
                            new Uri($"{applicationSection["BlazorClient:Uri"]}/authentication/login-callback")
                        },
                        Permissions =
                        {
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Logout,
                            Permissions.Endpoints.Token,
                            Permissions.GrantTypes.AuthorizationCode,
                            Permissions.GrantTypes.RefreshToken,
                            Permissions.ResponseTypes.Code,
                            Permissions.Scopes.Email,
                            Permissions.Scopes.Profile,
                            Permissions.Scopes.Roles,
                            $"{Permissions.Prefixes.Scope}{ScopeNameConstants.BLOGPOST_ALL_SERVICES}"
                        },
                        Requirements =
                        {
                            Requirements.Features.ProofKeyForCodeExchange
                        }
                    });
                }

                if (await manager.FindByClientIdAsync("mvc") is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "mvc",
                        ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
                        ConsentType = ConsentTypes.Explicit,
                        DisplayName = "MVC client application",
                        DisplayNames =
                        {
                            [CultureInfo.GetCultureInfo("fr-FR")] = "Application cliente MVC"
                        },
                        PostLogoutRedirectUris =
                        {
                            new Uri("https://localhost:44381/signout-callback-oidc")
                        },
                        RedirectUris =
                        {
                            new Uri("https://localhost:44381/signin-oidc")
                        },
                        Permissions =
                        {
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Logout,
                            Permissions.Endpoints.Token,
                            Permissions.GrantTypes.AuthorizationCode,
                            Permissions.GrantTypes.RefreshToken,
                            Permissions.ResponseTypes.Code,
                            Permissions.Scopes.Email,
                            Permissions.Scopes.Profile,
                            Permissions.Scopes.Roles,
                            $"{Permissions.Prefixes.Scope}demo_api"
                        },
                        Requirements =
                        {
                            Requirements.Features.ProofKeyForCodeExchange
                        }
                    });
                }

                // To test this sample with Postman, use the following settings:
                //
                // * Authorization URL: https://localhost:44395/connect/authorize
                // * Access token URL: https://localhost:44395/connect/token
                // * Client ID: postman
                // * Client secret: [blank] (not used with public clients)
                // * Scope: openid email profile roles
                // * Grant type: authorization code
                // * Request access token locally: yes
                if (await manager.FindByClientIdAsync("postman") is null)
                {
                    await manager.CreateAsync(new OpenIddictApplicationDescriptor
                    {
                        ClientId = "postman",
                        ConsentType = ConsentTypes.Systematic,
                        DisplayName = "Postman",
                        RedirectUris =
                        {
                            new Uri("urn:postman")
                        },
                        Permissions =
                        {
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Device,
                            Permissions.Endpoints.Token,
                            Permissions.GrantTypes.AuthorizationCode,
                            Permissions.GrantTypes.DeviceCode,
                            Permissions.GrantTypes.Password,
                            Permissions.GrantTypes.RefreshToken,
                            Permissions.ResponseTypes.Code,
                            Permissions.Scopes.Email,
                            Permissions.Scopes.Profile,
                            Permissions.Scopes.Roles
                        }
                    });
                }
            }

            static async Task RegisterScopesAsync(IServiceProvider provider)
            {
                var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

                if (await manager.FindByNameAsync("demo_api") is null)
                {
                    await manager.CreateAsync(new OpenIddictScopeDescriptor
                    {
                        DisplayName = "Demo API access",
                        DisplayNames =
                        {
                            [CultureInfo.GetCultureInfo("fr-FR")] = "Accès à l'API de démo"
                        },
                        Name = "demo_api",
                        Resources =
                        {
                            "resource_server"
                        }
                    });
                }

                if (await manager.FindByNameAsync(ScopeNameConstants.BLOGPOST_ALL_SERVICES) is null)
                {
                    await manager.CreateAsync(new OpenIddictScopeDescriptor
                    {
                        DisplayName = "Access all services of BlogPost services",
                        Name = ScopeNameConstants.BLOGPOST_ALL_SERVICES,
                        Resources =
                        {
                            ResourceNameConstants.BLOGPOST_RESOURCE
                        }
                    });
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}