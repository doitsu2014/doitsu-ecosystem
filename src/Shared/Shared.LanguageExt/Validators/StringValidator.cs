using System;
using LanguageExt;
using Shared.LanguageExt.ActionResults;
using Shared.LanguageExt.Common;
using Shared.LanguageExt.Models.Types;
using static LanguageExt.Prelude;

namespace Shared.LanguageExt.Validators
{
    public static class StringValidator
    {
        public static Validation<ErrorString, string> ShouldNotNullOrEmpty(this string value) =>
            !value.IsNullOrEmpty()
                ? Success<ErrorString, string>(value)
                : Fail<ErrorString, string>("value is null or empty");

        public static Validation<ErrorString, string> ShouldNotNullOrEmpty(this string value, string errorMessage) =>
            value.ShouldNotNullOrEmpty()
                .MapFail<ErrorString>(_ => errorMessage);

        public static Validation<ErrorString, string[]> ShouldNotNullOrEmpty(this string[] value) =>
            value is { Length: > 0 }
                ? Success<ErrorString, string[]>(value)
                : Fail<ErrorString, string[]>("list values is null or empty");

        public static Func<string, Validation<ErrorString, string>> MaxStrLength(int maxLength) =>
            fun((string value) =>
                (value != null)
                    ? maxLength >= 0
                        ? (value.Length <= maxLength)
                            ? Success<ErrorString, string>(value)
                            : Fail<ErrorString, string>($"value is greater than {maxLength}")
                        : Fail<ErrorString, string>($"{nameof(maxLength)} is less than zero")
                    : Fail<ErrorString, string>("value is null"));

        public static Func<string, Validation<ErrorString, string>> MinStrLength(int minLength) =>
            fun((string value) =>
                (value != null)
                    ? minLength >= 0
                        ? (value.Length >= minLength)
                            ? Success<ErrorString, string>(value)
                            : Fail<ErrorString, string>($"value is less than {minLength}")
                        : Fail<ErrorString, string>($"{nameof(minLength)} is less than zero")
                    : Fail<ErrorString, string>("value is null"));
    }
}