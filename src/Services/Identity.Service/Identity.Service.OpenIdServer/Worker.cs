using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            var configuration = provider.GetRequiredService<IConfiguration>();
            var applicationSection = configuration.GetSection("Initial:Application");

            var clientServiceGateway = "client.services.gateway";
            if (await manager.FindByClientIdAsync(applicationSection["ServiceGateway:ClientId"]) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    DisplayName = clientServiceGateway,
                    ClientId = applicationSection["ServiceGateway:ClientId"],
                    ClientSecret = applicationSection["ServiceGateway:ClientSecret"],
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials,
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeBlogPostRead}",
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeBlogPostWrite}"
                    },
                });
            }

            var clientBlazor = "client.blazor";
            if (await manager.FindByClientIdAsync(applicationSection["BlazorClient:ClientId"]) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = applicationSection["BlazorClient:ClientId"],
                    ConsentType = ConsentTypes.Explicit,
                    DisplayName = clientBlazor,
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
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeBlogPostRead}",
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeBlogPostWrite}"
                    },
                    Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
                });
            }
        }

        private async Task RegisterScopesAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictScopeManager>();

            var funcCreateScopeDescription = new Func<string, string[], OpenIddictScopeDescriptor>((
                (scopeName, resources) =>
                {
                    var scopeDescriptor = new OpenIddictScopeDescriptor
                    {
                        DisplayName = scopeName,
                        Name = scopeName,
                        Resources = { }
                    };

                    foreach (var resource in resources)
                    {
                        scopeDescriptor.Resources.Add(resource);
                    }

                    return scopeDescriptor;
                }));

            if (await manager.FindByNameAsync(ScopeNameConstants.ScopeBlogPostRead) is null)
            {
                await manager.CreateAsync(funcCreateScopeDescription(ScopeNameConstants.ScopeBlogPostRead, new string[]
                {
                    ResourceNameConstants.ResourceBlogPost,
                    ResourceNameConstants.ResourceBlogTag,
                    ResourceNameConstants.ResourceBlogComment,
                    ResourceNameConstants.ResourceBlogLike
                }));
            }

            if (await manager.FindByNameAsync(ScopeNameConstants.ScopeBlogPostWrite) is null)
            {
                await manager.CreateAsync(funcCreateScopeDescription(ScopeNameConstants.ScopeBlogPostWrite, new string[]
                {
                    ResourceNameConstants.ResourceBlogPost,
                    ResourceNameConstants.ResourceBlogTag,
                    ResourceNameConstants.ResourceBlogComment,
                    ResourceNameConstants.ResourceBlogLike
                }));
            }
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