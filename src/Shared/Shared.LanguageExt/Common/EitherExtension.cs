using System;
using System.Threading.Tasks;
using LanguageExt;
using Shared.LanguageExt.Models.Types;
using static LanguageExt.Prelude;

namespace Shared.LanguageExt.Common;

public static class EitherExtension
{
    public static Validation<ErrorString, T> Validate<T>(this Either<Seq<ErrorString>, Validation<ErrorString, T>> either)
        => either.Match(result => result,
            errors => errors.Join());

    public static Task<Validation<ErrorString, TMap>> MapAsync<T, TMap>(this Validation<ErrorString, T> value, Func<T, Task<TMap>> transformAsync)
    {
        return value.MatchAsync(async success => Success<ErrorString, TMap>(await transformAsync(success)), error => Fail<ErrorString, TMap>(error.Join()));
    }

    public static async Task<Validation<ErrorString, TMap>> MapAsync<T, TMap>(this Task<Validation<ErrorString, T>> taskValue, Func<T, Task<TMap>> transformAsync)
    {
        var value = await taskValue;
        return await value.MapAsync(transformAsync);
    }

    public static Task<Validation<ErrorString, TMap>> MapAsync<T, TMap>(this Validation<ErrorString, T> value, Func<T, Task<Validation<ErrorString, TMap>>> transformAsync)
    {
        return value.MatchAsync(async success => await transformAsync(success),
            error => Fail<ErrorString, TMap>(error.Join()));
    }
}