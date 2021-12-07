using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using Shared.LanguageExt.Common;

namespace Identity.Service.OpenIdServer.Services;

public interface IScopeService
{
    Task<Option<(string scopeName, string resources)>> CreateScopeAsync(OpenIddictScopeDescriptor descriptor);
}

public class ScopeService : IScopeService
{
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly ILogger<ScopeService> _logger;

    public ScopeService(IOpenIddictApplicationManager applicationManager,
        IOpenIddictScopeManager scopeManager,
        ILogger<ScopeService> logger)
    {
        _scopeManager = scopeManager;
        _logger = logger;
    }

    public async Task<Option<(string scopeName, string resources)>> CreateScopeAsync(OpenIddictScopeDescriptor descriptor)
    {
        if (await _scopeManager.FindByNameAsync(descriptor.Name) is null)
        {
            await _scopeManager.CreateAsync(descriptor);
            _logger.LogInformation("Created Scope {scopeName}.", descriptor.Name);
            return Option<(string scopeName, string resources)>.Some((descriptor.Name, descriptor.Resources.ToList().Join(", ")));
        }
        else
        {
            return Option<(string scopeName, string resources)>.None;
        }
    }
}