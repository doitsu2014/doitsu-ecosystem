using System;
using LanguageExt;
using Shared.Abstraction.Models.Types;
using Shared.Extensions;
using static LanguageExt.Prelude;

namespace Shared.Validations
{
    public static class StringValidator
    {
        public static Validation<Error, string> ShouldNotNullOrEmpty(string value) =>
            !value.IsNullOrEmpty()
                ? Success<Error, string>(value)
                : Fail<Error, string>("value is null or empty");

        public static Validation<Error, string[]> ShouldNotNullOrEmpty(string[] value) =>
            (value != null && value.Length > 0)
                ? Success<Error, string[]>(value)
                : Fail<Error, string[]>("list values is null or empty");

        public static Func<string, Validation<Error, string>> MaxStrLength(int maxLength) =>
            fun((string value) => 
                (value != null)
                    ? maxLength >= 0 
                        ? (value.Length <= maxLength)
                            ? Success<Error, string>(value)
                            : Fail<Error, string>($"value is greater than {maxLength}")
                        : Fail<Error, string>($"{nameof(maxLength)} is less than zero")
                    : Fail<Error, string>("value is null"));

        public static Func<string, Validation<Error, string>> MinStrLength(int minLength) =>
            fun((string value) =>
                (value != null) 
                    ? minLength >= 0 
                        ? (value.Length >= minLength)
                            ? Success<Error, string>(value)
                            : Fail<Error, string>($"value is less than {minLength}")
                        : Fail<Error, string>($"{nameof(minLength)} is less than zero")
                    : Fail<Error, string>("value is null"));
    }
}