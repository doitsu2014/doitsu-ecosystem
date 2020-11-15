using System.Threading.Tasks;

namespace Identity.Service.OpenIdServer.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
