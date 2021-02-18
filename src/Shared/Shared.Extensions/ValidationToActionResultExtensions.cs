using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstraction.Models.Types;

namespace Shared.Extensions
{
    public static class ValidationToActionResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Validation<Error, T> validation) =>
            validation.Match<IActionResult>(
                Succ: t => new OkObjectResult(t),
                Fail: e => new BadRequestObjectResult(e));

        public static Task<IActionResult> ToActionResultAsync<T>(this Task<Validation<Error, T>> validation) =>
            validation.Map(ToActionResult);

        public static Task<IActionResult> ToActionResultAsync(this Task<Validation<Error, Task>> validation) =>
            validation.Bind(ToActionResultAsync);
        
        private static Task<IActionResult> ToActionResultAsync(Validation<Error, Task> validation) =>
            validation.MatchAsync<IActionResult>(
                SuccAsync: async t => { await t; return new OkResult(); },
                Fail: e => new BadRequestObjectResult(e));
    }
}