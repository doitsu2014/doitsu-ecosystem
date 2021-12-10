using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using Shared.LanguageExt.Common;

namespace Identity.Service.OpenIdServer.Services;

public interface IApplicationService
{
    Task<Option<(string clientId, string displayName, string secret)>> CreateApplicationAsync(OpenIddictApplicationDescriptor descriptor);
}

public class ApplicationService : IApplicationService
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(IOpenIddictApplicationManager applicationManager,
        ILogger<ApplicationService> logger)
    {
        _applicationManager = applicationManager;
        _logger = logger;
    }

    public async Task<Option<(string clientId, string displayName, string secret)>> CreateApplicationAsync(OpenIddictApplicationDescriptor descriptor)
    {
        if (await _applicationManager.FindByClientIdAsync(descriptor.ClientId) is null)
        {
            descriptor.ClientSecret = descriptor.Type != OpenIddictConstants.ClientTypes.Public 
                ? descriptor.ClientSecret.IsNullOrEmpty()
                    ? new StringBuilder()
                        .AddRandomAlphabet(3)
                        .AddRandomSpecialString(3)
                        .AddRandomAlphabet(3, true)
                        .AddRandomNumber(10, 99)
                        .AddRandomSpecialString(3)
                        .ToString()
                    : descriptor.ClientSecret
                : null;

            await _applicationManager.CreateAsync(descriptor);
            _logger.LogInformation("Created Application Client Id {clientId}.", descriptor.ClientId);
            return Option<(string clientId, string displayName, string rawSecret)>.Some((descriptor.ClientId, descriptor.DisplayName, descriptor.ClientSecret));
        }
        else
        {
            return Option<(string clientId, string displayName, string rawSecret)>.None;
        }
    }
}