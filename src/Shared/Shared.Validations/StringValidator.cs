using LanguageExt;
using Shared.Abstraction;
using static LanguageExt.Prelude;

namespace Shared.Validations
{
    public static class StringValidator
    {
        public static Validation<string, string> ShouldNotNullOrEmpty(string value, string failMessage) => 
            !value.IsNullOrEmpty() 
                ? Success<string, string>(value)
                : Fail<string, string>(failMessage);
        
        public static Validation<string, string[]> ShouldNotNullOrEmpty(string[] value, string failMessage) => 
            (value != null && value.Length > 0)
                ? Success<string, string[]>(value)
                : Fail<string, string[]>(failMessage);
        
    }
}