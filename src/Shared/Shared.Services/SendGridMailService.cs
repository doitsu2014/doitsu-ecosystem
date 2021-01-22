using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Abstraction;
using Shared.Abstraction.Interfaces.Services;
using Shared.Abstraction.Model;
using Shared.Abstraction.Settings;
using Shared.Validations;

namespace Shared.Services
{
    public class SendGridMailService : IMailService
    {
        private readonly ILogger<SendGridSettings> _logger;
        private readonly SendGridSettings _settings;

        public SendGridMailService(IOptions<SendGridSettings> optSettings, ILogger<SendGridSettings> logger)
        {
            _logger = logger;
            _settings = optSettings.Value;
            (from vSetting in GenericValidator.ShouldNotNull(_settings)
                    from vApiKey in StringValidator.ShouldNotNullOrEmpty(_settings.ApiKey)
                    select (vSetting, vApiKey))
                .IfFail(errors => logger.LogWarning(errors.ComposeStrings(", ")));
        }

        public Task<Either<string, string>> SendMailAsync(SendingMailDescriptor data)
        {
            throw new System.NotImplementedException();
        }

        public Task<Either<string, string>> SendMailsAsync(IList<SendingMailDescriptor> data)
        {
            throw new System.NotImplementedException();
        }
    }
}