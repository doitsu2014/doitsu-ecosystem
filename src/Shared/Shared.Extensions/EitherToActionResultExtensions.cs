using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstraction.Models.Types;

namespace Shared.Extensions
{
    public static class EitherToActionResultExtensions
    {
        public static Task<IActionResult> ToActionResultAsync<L, R>(this Task<Either<L, R>> either) =>
            either.Map(Match);

        public static Task<IActionResult> ToActionResultAsync(this Task<Either<Error, Task>> either) =>
            either.Bind(MatchAsync);

        private static IActionResult Match<L, R>(this Either<L, R> either) =>
            either.Match<IActionResult>(
                Left: l => new BadRequestObjectResult(l),
                Right: r => new OkObjectResult(r));

        private static async Task<IActionResult> MatchAsync(Either<Error, Task> either) =>
            await either.MatchAsync<IActionResult>(
                RightAsync: async t => { await t; return new OkResult(); },
                Left: e => new BadRequestObjectResult(e));
        
    }
}