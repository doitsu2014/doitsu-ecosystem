using LanguageExt;
using Shared.Abstraction;
using static LanguageExt.Prelude;

namespace Shared.Validations
{
    public static class StringValidator
    {
        public static Validation<string, string> ShouldNotNullOrEmpty(string value) => 
            !value.IsNullOrEmpty() 
                ? Success<string, string>(value)
                : Fail<string, string>("");
    }
}