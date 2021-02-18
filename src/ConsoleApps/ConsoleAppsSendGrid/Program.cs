using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;
using SendGrid.Helpers.Mail;
using Shared.Abstraction.Settings;
using Shared.ConsoleApp.Extension;

namespace Shared.ConsoleApp.SendGrid
{
    public class Program
    {
        public static async Task Main(string[] args) => await ServiceProviderFactory.GetServiceProvider(
            addCustomServices: (sc, conf) =>
            {
                sc.Configure<SendGridSettings>(conf.GetSection("SendGridSettings"));
                sc.AddSendGrid(sgOpt =>
                {
                    sgOpt.ApiKey = conf["SendGridSettings:ApiKey"];
                });
            },
            jsonFilePath: "Configurations/appSettings.json",
            assembly: Assembly.GetAssembly(typeof(Program))
        ).MatchAsync(
            async sp =>
            {
                using var scope = sp.CreateScope();
                var sendGridClient = scope.ServiceProvider.GetRequiredService<ISendGridClient>();
                var from = new EmailAddress("duc.tran@doitsu.tech", "Duc Tran");
                var subject = "Sending with SendGrid is Fun";
                var to = new EmailAddress("target@doitsu.tech", "Target User");
                var plainTextContent = "and easy to do anywhere, even with C#";
                var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await sendGridClient.SendEmailAsync(msg); 
                return true;
            }, () => false);
    }
}