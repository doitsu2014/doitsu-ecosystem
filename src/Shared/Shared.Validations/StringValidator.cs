using System;
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

        public static Func<string, Validation<string, string>> MaxStrLength(int maxLength) =>
            fun((string value) => 
                (value != null)
                    ? maxLength >= 0 
                        ? (value.Length <= maxLength)
                            ? Success<string, string>(value)
                            : Fail<string, string>($"value is greater than {maxLength}")
                        : Fail<string, string>($"{nameof(maxLength)} is less than zero.")
                    : Fail<string, string>("value is null."));

        public static Func<string, Validation<string, string>> MinStrLength(int minLength) =>
            fun((string value) =>
                (value != null) 
                    ? minLength >= 0 
                        ? (value.Length >= minLength)
                            ? Success<string, string>(value)
                            : Fail<string, string>($"value is less than {minLength}")
                        : Fail<string, string>($"{nameof(minLength)} is less than zero.")
                    : Fail<string, string>("value is null."));
    }
}