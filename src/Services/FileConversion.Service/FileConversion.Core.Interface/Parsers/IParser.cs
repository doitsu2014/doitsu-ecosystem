using FileConversion.Abstraction.Model.StandardV2;
using System.Collections.Immutable;
using LanguageExt;
using Shared.Abstraction.Models.Types;

namespace FileConversion.Core.Interface.Parsers
{
    public interface IParser<T> where T : IStandardModel
    {
        Either<Error, ImmutableList<T>> Parse(byte[] content);
    }

    public interface ICustomParser
    {
        string Key { get; }
    }

    public interface ICustomParser<T> : IParser<T>, ICustomParser where T : IStandardModel { }
}
