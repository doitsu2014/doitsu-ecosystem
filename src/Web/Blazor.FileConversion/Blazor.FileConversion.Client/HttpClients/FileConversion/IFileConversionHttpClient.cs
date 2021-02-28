using System.Collections.Immutable;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.FileConversion.Client.Models;

namespace Blazor.FileConversion.Client.HttpClients.FileConversion
{
    public interface IFileConversionHttpClient
    {
        Task<ImmutableList<PackagingDocument>> ParsePackagingDocumentAsync(MultipartFormDataContent content);
    }
}