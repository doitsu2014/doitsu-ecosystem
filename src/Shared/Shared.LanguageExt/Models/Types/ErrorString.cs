using System.Collections.Generic;
using LanguageExt;

namespace Shared.LanguageExt.Models.Types
{
    public class ErrorString : NewType<ErrorString, string>
    {
        public ErrorString(string str) : base(str) { }
        public static implicit operator ErrorString(string str) => New(str);
    }

    public static class ErrorExtensions
    {
        public static ErrorString Join(this Seq<ErrorString> errors) => string.Join("; ", errors);
        public static ErrorString Join(this IEnumerable<ErrorString> errors) => string.Join("; ", errors);
    }
}