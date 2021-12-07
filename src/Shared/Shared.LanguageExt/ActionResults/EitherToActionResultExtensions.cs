using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Shared.LanguageExt.Models.Types;

namespace Shared.LanguageExt.ActionResults
{
    public static class EitherToActionResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Either<ErrorString, T> either) =>
            either.Match<IActionResult>(
                t => new OkObjectResult(t),
                e => new BadRequestObjectResult(e));

        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Either<ErrorString, T>> either) =>
            ToActionResult(await either);

        public static Task<IActionResult> ToActionResultAsync<T>(this Either<ErrorString, Task<T>> either)
        {
            return either.MatchAsync<IActionResult>(async x => new OkObjectResult(await x),
                err => new BadRequestObjectResult(err));
        }

        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Either<ErrorString, Task<T>>> either)
        {
            return await ToActionResultAsync(await either);
        }

        public static Task<IActionResult> ToActionResultAsync(this Task<Either<ErrorString, Task>> either) =>
            either.Bind(ToActionResultAsync);

        private static Task<IActionResult> ToActionResultAsync(Either<ErrorString, Task> either) =>
            either.MatchAsync<IActionResult>(
                async t =>
                {
                    await t;
                    return new OkResult();
                },
                e => new BadRequestObjectResult(e));
    }
}