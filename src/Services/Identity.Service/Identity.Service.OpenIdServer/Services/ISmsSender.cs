using System.Threading.Tasks;

namespace Identity.Service.OpenIdServer.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
