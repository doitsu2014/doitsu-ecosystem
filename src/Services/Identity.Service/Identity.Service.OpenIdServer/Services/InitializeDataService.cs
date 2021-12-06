using System;
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
using Shared.Extensions;

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

    public InitializeDataService(IServiceProvider sp)
    {
        var scope = sp.CreateScope();
        _dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        _logger = scope.ServiceProvider.GetService<ILogger<InitializeDataService>>();
        _initialSetting = scope.ServiceProvider.GetService<IOptions<InitialSetting>>().Value;
        _applicationService = scope.ServiceProvider.GetService<IApplicationService>();
        _minIOService = scope.ServiceProvider.GetService<IMinIOService>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

        var addApplicationsResult = await AddApplicationsAsync();
        addApplicationsResult.IfSome(async result =>
        {
            var dateTimeString = DateTime.UtcNow.ToString("yyyyMMddHHmm");
            await _minIOService.UploadFileAsync(Path.Combine(MinIOExportFolder, $"new-settings-{dateTimeString}.txt"), result.ToBytes());
        });
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
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeImageServerRead}",
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeImageServerWrite}",
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeFileConversionAll}",
                $"{OpenIddictConstants.Permissions.Prefixes.Scope}{ScopeNameConstants.ScopeIdentityServerAllServices}",
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


    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}