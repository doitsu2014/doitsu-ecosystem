using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstraction.Models.Types;

namespace Shared.Extensions
{
    public static class EitherToActionResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Either<Error, T> either) =>
            either.Match<IActionResult>(
                t => new OkObjectResult(t),
                e => new BadRequestObjectResult(e));

        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Either<Error, T>> either) =>
            ToActionResult(await either);

        public static Task<IActionResult> ToActionResultAsync<T>(this Either<Error, Task<T>> either)
        {
            return either.MatchAsync<IActionResult>(async x => new OkObjectResult(await x),
                err => new BadRequestObjectResult(err));
        }

        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Either<Error, Task<T>>> either)
        {
            return await ToActionResultAsync(await either);
        }

        public static Task<IActionResult> ToActionResultAsync(this Task<Either<Error, Task>> either) =>
            either.Bind(ToActionResultAsync);

        private static Task<IActionResult> ToActionResultAsync(Either<Error, Task> either) =>
            either.MatchAsync<IActionResult>(
                async t =>
                {
                    await t;
                    return new OkResult();
                },
                e => new BadRequestObjectResult(e));
    }
}