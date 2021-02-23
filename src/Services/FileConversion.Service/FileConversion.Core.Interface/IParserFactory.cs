using FileConversion.Abstraction.Model.StandardV2;
using FileConversion.Core.Interface.Parsers;
using Optional;
using System.Threading.Tasks;

namespace FileConversion.Core.Interface
{
    public interface IParserFactory
    {
        Task<Option<IParser<T>, string>> GetParserAsync<T>(string key) where T : IStandardModel;
    }
}
