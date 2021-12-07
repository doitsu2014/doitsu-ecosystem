using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Identity.Service.OpenIdServer.Constants;
using Identity.Service.OpenIdServer.Data;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using Shared.LanguageExt.Common;

namespace Identity.Service.OpenIdServer.Services;

public class InitializeDataService : IHostedService
{
    private readonly string MinIOExportFolder = "secret-data";
    private readonly ILogger<InitializeDataService> _logger;
    private readonly InitialSetting _initialSetting;
    private readonly IConfiguration _configuration;
    private readonly IApplicationService _applicationService;
    private readonly IMinIOService _minIOService;
    private readonly ApplicationDbContext _dbContext;
    private readonly IScopeService _scopeService;

    public InitializeDataService(IServiceProvider sp)
    {
        var scope = sp.CreateScope();
        _dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        _logger = scope.ServiceProvider.GetService<ILogger<InitializeDataService>>();
        _initialSetting = scope.ServiceProvider.GetService<IOptions<InitialSetting>>().Value;
        _applicationService = scope.ServiceProvider.GetService<IApplicationService>();
        _scopeService = scope.ServiceProvider.GetService<IScopeService>();
        _minIOService = scope.ServiceProvider.GetService<IMinIOService>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);

        var addApplicationsResult = await AddApplicationsAsync();
        var addScopesResult = await AddScopesAsync();
        var newSettingContent = new[] { addApplicationsResult, addScopesResult }
            .Somes()
            .Join("\r\n\r\n");

        if (!newSettingContent.IsNullOrEmpty())
        {
            var dateTimeString = DateTime.UtcNow.ToString("yyyyMMddHHmm");
            await _minIOService.UploadFileAsync(Path.Combine(MinIOExportFolder, $"new-settings-{dateTimeString}.txt"), newSettingContent.ToBytes());
        }
        else
        {
            _logger.LogInformation("No obtain any new setting");
        }
    }

    /// <summary>
    /// Return report string
    /// </summary>
    /// <returns></returns>
    public async Task<Option<string>> AddApplicationsAsync()
    {
        var listResult = ImmutableList<Option<(string clientId, string displayName, string secret)>>.Empty;
        listResult = listResult.Add(await _applicationService.CreateApplicationAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = _initialSetting.Administrator.ClientId,
            DisplayName = _initialSetting.Administrator.DisplayName,
            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
            },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Introspection,
                OpenIddictConstants.Permissions.GrantTypes.Password,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Address,
                OpenIddictConstants.Permissions.Scopes.Roles,
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeImageServerAll}",
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeIdentityServerAll}",
            }
        }));

        if (listResult.Somes().Any())
        {
            return Option<string>.Some(listResult.Somes()
                .Select(s => $"Added client {s.displayName}, clientId: {s.clientId}, clientSecret: {s.secret}")
                .Aggregate((a, b) => $"- {a}\n- {b}"));
        }

        return Option<string>.None;
    }

    private async Task<Option<string>> AddScopesAsync()
    {
        var listResult = ImmutableList<Option<(string scopeName, string resources)>>.Empty;

        var resources = new Dictionary<string, string[]>();
        resources.Add(ResourceNameConstants.ResourceImageServer, new[]
        {
            ScopeNameConstants.ScopeImageServerRead, ScopeNameConstants.ScopeImageServerWrite, ScopeNameConstants.ScopeImageServerAll
        });
        resources.Add(ResourceNameConstants.ResourceIdentityServer, new[]
        {
            ScopeNameConstants.ScopeIdentityServerAll, ScopeNameConstants.ScopeIdentityServerUserInfo
        });
        resources.Add(ResourceNameConstants.ResourcePosts, new[]
        {
            ScopeNameConstants.ScopePostsAll, ScopeNameConstants.ScopePostsRead, ScopeNameConstants.ScopePostsWrite
        });
        resources.Add(ResourceNameConstants.ResourceFileConversion, new[]
        {
            ScopeNameConstants.ScopeFileConversionAll, ScopeNameConstants.ScopeFileConversionRead, ScopeNameConstants.ScopeFileConversionWrite, ScopeNameConstants.ScopeFileConversionParse
        });

        var scopes = resources.SelectMany(kv => kv.Value.Select(s => new OpenIddictScopeDescriptor()
        {
            Name = s,
            DisplayName = s,
            Resources = { kv.Key }
        }));

        foreach (var s in scopes)
        {
            listResult = listResult.Add(await _scopeService.CreateScopeAsync(s));
        }

        if (listResult.Somes().Any())
        {
            return Option<string>.Some(listResult.Somes()
                .Select(s => $"Added scope {s.scopeName} to resources [{s.resources}]")
                .Join("\r\n"));
        }

        return Option<string>.None;
    }

    private async Task AddUsersAsync()
    {
        // var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        // var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        // var configuration = provider.GetRequiredService<IConfiguration>();
        // var adminUserSection = configuration.GetSection("InitialSetting:AdminUser");
        //
        // if (!userManager.Users.Any())
        // {
        //     var adminUser = new ApplicationUser()
        //     {
        //         Id = Guid.NewGuid().ToString(),
        //         Email = adminUserSection["EmailAddress"],
        //         NormalizedEmail = adminUserSection["EmailAddress"].ToUpper(),
        //         UserName = adminUserSection["EmailAddress"],
        //         NormalizedUserName = adminUserSection["EmailAddress"].ToUpper(),
        //         City = "HCM",
        //         State = "HCM",
        //         Country = "VN",
        //         Name = "TRAN HUU DUC",
        //         PhoneNumber = "0946680600"
        //     };
        //     await userManager.CreateAsync(adminUser, adminUserSection["Password"]);
        //
        //     var listRoleNames = (new string[]
        //     {
        //         IdentityRoleConstants.Admin,
        //         IdentityRoleConstants.Customer,
        //         IdentityRoleConstants.BlogManager,
        //         IdentityRoleConstants.BlogPublisher,
        //         IdentityRoleConstants.BlogWriter
        //     });
        //
        //     if (!roleManager.Roles.Any())
        //     {
        //         var roles = listRoleNames.Select(r => new IdentityRole()
        //             { Id = Guid.NewGuid().ToString(), Name = r, NormalizedName = r.ToUpper() });
        //         foreach (var role in roles) await roleManager.CreateAsync(role);
        //     }
        //
        //     await userManager.AddToRolesAsync(adminUser, listRoleNames);
        }
    }


    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}