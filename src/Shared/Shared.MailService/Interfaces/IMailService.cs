using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using Shared.MailService.Models;

namespace Shared.MailService.Interfaces
{
    public interface IMailService
    {
        Task<Either<string, string>> SendMailAsync(SendingMailDescriptor data);
        Task<Either<string, string>> SendMailsAsync(IList<SendingMailDescriptor> data);
    }
}