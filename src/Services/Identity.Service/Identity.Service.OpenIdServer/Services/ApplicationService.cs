using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using Shared.Extensions;

namespace Identity.Service.OpenIdServer.Services;

public interface IApplicationService
{
    Task<Option<(string clientId, string displayName, string secret)>> CreateApplicationAsync(OpenIddictApplicationDescriptor descriptor);
}

public class ApplicationService : IApplicationService
{
    private readonly IOpenIddictApplicationManager _manager;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(IOpenIddictApplicationManager manager, ILogger<ApplicationService> logger)
    {
        _manager = manager;
        _logger = logger;
    }
    
    public async Task<Option<(string clientId, string displayName, string secret)>> CreateApplicationAsync(OpenIddictApplicationDescriptor descriptor)
    {
        if (await _manager.FindByClientIdAsync(descriptor.ClientId) is null)
        {
            descriptor.ClientSecret = (new StringBuilder())
                .AddRandomAlphabet(3)
                .AddRandomSpecialString(3)
                .AddRandomAlphabet(3, true)
                .AddRandomNumber(10, 99)
                .AddRandomSpecialString(3)
                .ToString();
            
            await _manager.CreateAsync(descriptor);
            _logger.LogInformation("Created Application Client Id {clientId}.", descriptor.ClientId);
            return Option<(string clientId, string displayName, string rawSecret)>.Some((descriptor.ClientId, descriptor.DisplayName, descriptor.ClientSecret));
        }
        else
        {
            return Option<(string clientId, string displayName, string rawSecret)>.None;
        }
    }
}