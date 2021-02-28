using System.Collections.Immutable;
using System.Threading.Tasks;
using LanguageExt;
using Shared.Abstraction.Models.Types;

namespace FileConversion.Core.Interface
{
    public interface IExportService
    {
        Task<Either<Error, byte[]>> ExportAsync(string inputType, ImmutableList<object> data);
    }
}
