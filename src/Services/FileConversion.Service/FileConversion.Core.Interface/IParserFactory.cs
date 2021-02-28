using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface.Parsers;
using System.Threading.Tasks;
using LanguageExt;
using Shared.Abstraction.Models.Types;

namespace FileConversion.Core.Interface
{
    public interface IParserFactory
    {
        Task<Either<Error, IParser<T>>> GetParserAsync<T>(string key) where T : IStandardModel;
    }
}
