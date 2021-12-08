using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Service.OpenIdServer.Constants;
using Identity.Service.OpenIdServer.Data;
using Identity.Service.OpenIdServer.Settings;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
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
    private readonly IApplicationService _applicationService;
    private readonly IMinIOService _minIOService;
    private readonly ApplicationDbContext _dbContext;
    private readonly IScopeService _scopeService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public InitializeDataService(IServiceProvider sp)
    {
        var scope = sp.CreateScope();
        _dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
        _logger = scope.ServiceProvider.GetService<ILogger<InitializeDataService>>();
        _initialSetting = scope.ServiceProvider.GetService<IOptions<InitialSetting>>().Value;
        _applicationService = scope.ServiceProvider.GetService<IApplicationService>();
        _scopeService = scope.ServiceProvider.GetService<IScopeService>();
        _userService = scope.ServiceProvider.GetService<IUserService>();
        _minIOService = scope.ServiceProvider.GetService<IMinIOService>();
        _mapper = scope.ServiceProvider.GetService<IMapper>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);

        var addApplicationsResult = await AddApplicationsAsync();
        var addScopesResult = await AddScopesAsync();
        var addUsersResult = await AddUsersAsync();
        var newSettingContent = new[] { addApplicationsResult, addScopesResult, addUsersResult }
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
        foreach (var application in _initialSetting.Applications)
        {
            listResult = listResult.Add(await _applicationService.CreateApplicationAsync(_mapper.Map<OpenIddictApplicationDescriptor>(application)));
        }

        if (listResult.Somes().Any())
        {
            return Option<string>.Some(listResult.Somes()
                .Select(s => $"- Added client {s.displayName}, clientId: {s.clientId}, clientSecret: {s.secret}")
                .Join("\r\n"));
        }

        return Option<string>.None;
    }

    private async Task<Option<string>> AddScopesAsync()
    {
        var listResult = ImmutableList<Option<(string scopeName, string resources)>>.Empty;
        var scopes = _initialSetting.Resources.SelectMany(resource => resource.Scopes.Select(s => new OpenIddictScopeDescriptor()
        {
            Name = s,
            DisplayName = s,
            Resources = { resource.AudienceName }
        }));

        foreach (var s in scopes)
        {
            listResult = listResult.Add(await _scopeService.CreateScopeAsync(s));
        }

        if (listResult.Somes().Any())
        {
            return Option<string>.Some(listResult.Somes()
                .Select(s => $"- Added scope {s.scopeName} to resources [{s.resources}]")
                .Join("\r\n"));
        }

        return Option<string>.None;
    }

    private async Task<Option<string>> AddUsersAsync()
    {
        var listResult = ImmutableList<Option<(string id, string email, string password, string roles)>>.Empty;
        foreach (var user in _initialSetting.Users)
        {
            listResult = listResult.Add(await _userService.CreateUserWithRolesAsync(user));
        }

        if (listResult.Somes().Any())
        {
            return Option<string>.Some(listResult
                .Somes()
                .Select(u => $"- Added user [{u.email} | {u.id}] with password {u.password}, and assigned her/him to roles [{u.roles}]")
                .Join("\r\n"));
        }

        return Option<string>.None;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}