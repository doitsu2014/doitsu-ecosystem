using System.Net.Sockets;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Identity.Service.OpenIdServer.Constants;
using Identity.Service.OpenIdServer.Data;
using Identity.Service.OpenIdServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            await context.Database.MigrateAsync(cancellationToken);

            await RegisterApplicationsAsync(scope.ServiceProvider);
            await RegisterScopesAsync(scope.ServiceProvider);
            await RegisterDefaultUsersAsync(scope.ServiceProvider);
        }

        private async Task RegisterApplicationsAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();
            var logger = provider.GetRequiredService<ILogger<Worker>>();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var applicationSection = configuration.GetSection("Initial:Application");

            var funcCreateClientAsync = new Func<string, string, OpenIddictApplicationDescriptor, Task>(
                async (clientId, clientDisplayName, data) =>
                {
                    if (await manager.FindByClientIdAsync(clientId) is null)
                    {
                        data.ClientId = clientId;
                        data.DisplayName = clientDisplayName;
                        await manager.CreateAsync(data);
                        logger.LogInformation("Created Application Client Id {clientId}.", clientId);
                    }
                    else
                    {
                        logger.LogInformation("Application Client Id {clientId} does exist, so bypass this case.",
                            clientId);
                    }
                });

            await funcCreateClientAsync(applicationSection["Administrator:ClientId"],
                "application.administrator",
                new OpenIddictApplicationDescriptor
                {
                    ClientSecret = applicationSection["Administrator:ClientSecret"],
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Introspection,
                        Permissions.GrantTypes.Password,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Address,
                        Permissions.Scopes.Roles,
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeImageServerRead}",
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeImageServerWrite}",
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeFileConversionAll}",
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeIdentityServerAllServices}",
                    }
                });

            await funcCreateClientAsync(applicationSection["FileConversion:ClientId"],
                "application.fileconversion",
                new OpenIddictApplicationDescriptor
                {
                    ConsentType = ConsentTypes.Explicit,
                    Type = ClientTypes.Public,
                    PostLogoutRedirectUris =
                    {
                        new Uri($"{applicationSection["FileConversion:Uri"]}/authentication/logout-callback")
                    },
                    RedirectUris =
                    {
                        new Uri($"{applicationSection["FileConversion:Uri"]}/authentication/login-callback")
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
                        Permissions.Scopes.Address,
                        Permissions.Scopes.Roles,
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeImageServerRead}",
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeImageServerWrite}",
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeFileConversionParse}",
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeIdentityServerUserInfo}"
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
        }

        private async Task RegisterScopesAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

            var funcCreateScopeDescriptorAsync = new Func<string, OpenIddictScopeDescriptor, Task>(
                async (scopeName, data) =>
                {
                    if (await manager.FindByNameAsync(scopeName) is null)
                    {
                        data.Name = scopeName;
                        data.DisplayName = scopeName;
                        await manager.CreateAsync(data);
                    }
                });

            var serviceFileConversionScopes = new string[]
            {
                ScopeNameConstants.ScopeFileConversionAll,
                ScopeNameConstants.ScopeFileConversionRead,
                ScopeNameConstants.ScopeFileConversionParse,
                ScopeNameConstants.ScopeFileConversionWrite
            };
            foreach (var scope in serviceFileConversionScopes)
            {
                await funcCreateScopeDescriptorAsync(scope,
                    new OpenIddictScopeDescriptor()
                    {
                        Resources =
                        {
                            ResourceNameConstants.ResourceFileConversion
                        }
                    });
            }

            await funcCreateScopeDescriptorAsync(ScopeNameConstants.ScopeImageServerRead,
                new OpenIddictScopeDescriptor()
                {
                    Resources =
                    {
                        ResourceNameConstants.ResourceImageServer
                    }
                });

            await funcCreateScopeDescriptorAsync(ScopeNameConstants.ScopeImageServerWrite,
                new OpenIddictScopeDescriptor()
                {
                    Resources =
                    {
                        ResourceNameConstants.ResourceImageServer
                    }
                });

            await funcCreateScopeDescriptorAsync(ScopeNameConstants.ScopeIdentityServerAllServices,
                new OpenIddictScopeDescriptor()
                {
                    Resources =
                    {
                        ResourceNameConstants.ResourceIdentityServer
                    }
                });

            await funcCreateScopeDescriptorAsync(ScopeNameConstants.ScopeIdentityServerUserInfo,
                new OpenIddictScopeDescriptor()
                {
                    Resources =
                    {
                        ResourceNameConstants.ResourceIdentityServer
                    }
                });
        }

        private async Task RegisterDefaultUsersAsync(IServiceProvider provider)
        {
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var adminUserSection = configuration.GetSection("Initial:AdminUser");

            if (!userManager.Users.Any())
            {
                var adminUser = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = adminUserSection["EmailAddress"],
                    NormalizedEmail = adminUserSection["EmailAddress"].ToUpper(),
                    UserName = adminUserSection["EmailAddress"],
                    NormalizedUserName = adminUserSection["EmailAddress"].ToUpper(),
                    City = "HCM",
                    State = "HCM",
                    Country = "VN",
                    Name = "TRAN HUU DUC",
                    PhoneNumber = "0946680600"
                };
                await userManager.CreateAsync(adminUser, adminUserSection["Password"]);
                
                var listRoleNames = (new string[]
                {
                    IdentityRoleConstants.Admin,
                    IdentityRoleConstants.Customer,
                    IdentityRoleConstants.BlogManager,
                    IdentityRoleConstants.BlogPublisher,
                    IdentityRoleConstants.BlogWriter
                });

                if (!roleManager.Roles.Any())
                {
                    var roles = listRoleNames.Select(r => new IdentityRole()
                        {Id = Guid.NewGuid().ToString(), Name = r, NormalizedName = r.ToUpper()});
                    foreach (var role in roles) await roleManager.CreateAsync(role);
                }

                await userManager.AddToRolesAsync(adminUser, listRoleNames);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}