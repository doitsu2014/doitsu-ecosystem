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
            await RegisterDefaultUsersAsync(scope.ServiceProvider);
        }

        private async Task RegisterApplicationsAsync(IServiceProvider provider)
        {
            var manager = provider.GetRequiredService<IOpenIddictApplicationManager>();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var applicationSection = configuration.GetSection("Initial:Application");

            var clientBlazor = "client.blazor";
            if (await manager.FindByClientIdAsync(clientBlazor) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = clientBlazor,
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
                        $"{Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeBlogPostAll}"
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
            if (await manager.FindByNameAsync(ResourceNameConstants.ResourceBlogPost) is null)
            {
                await manager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    DisplayName = ResourceNameConstants.ResourceBlogPost,
                    Name = ResourceNameConstants.ResourceBlogPost,
                    Resources =
                    {
                        ScopeNameConstants.ScopeBlogPostAll,
                        ScopeNameConstants.ScopeBlogPostCreate,
                        ScopeNameConstants.ScopeBlogPostUpdate,
                        ScopeNameConstants.ScopeBlogPostDelete,
                        ScopeNameConstants.ScopeBlogPostSearch
                    }
                });
            }
        }

        private async Task RegisterDefaultUsersAsync(IServiceProvider provider)
        {
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var adminUserSection = configuration.GetSection("Initial:AdminUser");

            if (userManager.Users.Count() == 0)
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
                    IdentityRoleConstants.ADMIN,
                    IdentityRoleConstants.CUSTOMER,
                    IdentityRoleConstants.BLOG_MANAGER,
                    IdentityRoleConstants.BLOG_PUBLISHER,
                    IdentityRoleConstants.BLOG_WRITER
                });
                if (roleManager.Roles.Count() == 0)
                {
                    var roles = listRoleNames.Select(r => new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = r, NormalizedName = r.ToUpper() });
                    foreach (var role in roles) await roleManager.CreateAsync(role);
                }

                await userManager.AddToRolesAsync(adminUser, listRoleNames);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}