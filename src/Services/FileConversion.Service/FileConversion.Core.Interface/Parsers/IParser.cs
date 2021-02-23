using FileConversion.Abstraction.Model.StandardV2;
using Optional;
using System.Collections.Immutable;

namespace FileConversion.Core.Interface.Parsers
{
    public interface IParser<T> where T : IStandardModel
    {
        Option<ImmutableList<T>, string> Parse(byte[] content);
    }

    public interface ICustomParser
    {
        string Key { get; }
    }

    public interface ICustomParser<T> : IParser<T>, ICustomParser where T : IStandardModel { }
}
