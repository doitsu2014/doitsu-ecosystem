using FileConversion.Abstraction;
using FileConversion.Abstraction.Model;
using Optional;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace FileConversion.Core.Interface
{
    public interface IExportService
    {
        Task<Option<byte[], string>> ExportAsync(string inputType, ImmutableList<object> data);
    }
}
