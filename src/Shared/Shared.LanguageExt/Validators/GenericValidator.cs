using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using Shared.LanguageExt.Models.Types;
using static LanguageExt.Prelude;

namespace Shared.LanguageExt.Validators;

public static class GenericValidator
{
    public static Validation<ErrorString, T[]> ShouldNotNullOrEmpty<T>(this T[] value) =>
        (value != null && value.Length > 0)
            ? Success<ErrorString, T[]>(value)
            : Fail<ErrorString, T[]>("value is null or empty");

    public static Validation<ErrorString, IList<T>> ShouldNotNullOrEmpty<T>(this IList<T> value) =>
        (value != null && value.Count > 0)
            ? Success<ErrorString, IList<T>>(value)
            : Fail<ErrorString, IList<T>>("value is null or empty");

    public static Validation<ErrorString, ICollection<T>> ShouldNotNullOrEmpty<T>(this ICollection<T> value) =>
        (value != null && value.Count > 0)
            ? Success<ErrorString, ICollection<T>>(value)
            : Fail<ErrorString, ICollection<T>>("value is null or empty");

    public static Validation<ErrorString, IEnumerable<T>> ShouldNotNullOrEmpty<T>(this IEnumerable<T> value) =>
        (value != null && value.Length() > 0)
            ? Success<ErrorString, IEnumerable<T>>(value)
            : Fail<ErrorString, IEnumerable<T>>("value is null or empty");

    public static Validation<ErrorString, T> ShouldNotNull<T>(this T value) =>
        value != null
            ? Success<ErrorString, T>(value)
            : Fail<ErrorString, T>("value is null");

    public static Validation<ErrorString, T> ShouldNotNull<T>(this T value, string errorMessage)
        => value.ShouldNotNull().MapFail<ErrorString>(_ => errorMessage);

    public static async ValueTask<Validation<ErrorString, T>> ShouldNotNullAsync<T>(this ValueTask<T> value, string errorMessage)
        => (await value).ShouldNotNull().MapFail<ErrorString>(_ => errorMessage);
}