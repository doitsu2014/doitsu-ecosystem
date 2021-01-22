using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Shared.Abstraction;
using Shared.Abstraction.Interfaces.Services;
using Shared.Abstraction.Model;
using Shared.Abstraction.Settings;
using Shared.Validations;
using static LanguageExt.Prelude;

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

        public async Task<Either<string, string>> SendMailAsync(SendingMailDescriptor data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var client = new SendGridClient(_settings.ApiKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress(data.From),
                Subject = data.Subject,
                HtmlContent = data.Content
            };

            var emails = new List<string>();
            new List<(List<string>, Action<List<string>>)>
            {
                (data.ToList, values => values.ForEach(v => msg.AddTo(v))),
                (data.CcList, values => msg.AddCcs(values.Select(v => new EmailAddress {Email = v}).ToList())),
                (data.BccList, values => msg.AddBccs(values.Select(v => new EmailAddress {Email = v}).ToList()))
            }.ForEach(element =>
            {
                if (element.Item1 != null && element.Item1.Any())
                {
                    element.Item2(element.Item1);
                    emails.AddRange(element.Item1);
                }
            });

            if (!emails.Any()) return Left<string, string>("Empty emails");
            _logger.LogDebug("Sending email to {Emails}", emails.Aggregate((x, y) => $"{x}, {y}"));

            if (data.Attachments != null && data.Attachments.Any())
            {
                msg.Attachments = data.Attachments.Select(at => new Attachment
                {
                    ContentId = Guid.NewGuid().ToString(),
                    Disposition = "inline",
                    Filename = at.FileName,
                    Content = Convert.ToBase64String(at.Content)
                }).ToList();
            }

            var response = await client.SendEmailAsync(msg);
            var statusGroup = (int) response.StatusCode / 100;
            if (statusGroup == 4 || statusGroup == 5)
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new ApplicationException($"Failed to send email.\n{body}");
            }

            return Right<string, string>("Sent mails");
        }

        public Task<Either<string, string>> SendMailsAsync(IList<SendingMailDescriptor> data)
        {
            throw new System.NotImplementedException();
        }
    }
}