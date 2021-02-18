using System.Collections.Generic;
using System.Runtime.Serialization;
using LanguageExt;

namespace Shared.Abstraction.Models.Types
{
    public class Error : NewType<Error, string>
    {
        public Error(string str) : base(str) { }
        public static implicit operator Error(string str) => New(str);
    }

    public static class ErrorExtensions
    {
        public static Error Join(this Seq<Error> errors) => string.Join("; ", errors);
        public static Error Join(this IEnumerable<Error> errors) => string.Join("; ", errors);
    }
}