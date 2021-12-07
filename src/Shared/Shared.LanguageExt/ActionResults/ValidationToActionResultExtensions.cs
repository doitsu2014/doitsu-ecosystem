using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Shared.LanguageExt.Models.Types;

namespace Shared.LanguageExt.ActionResults
{
    public static class ValidationToActionResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Validation<ErrorString, T> validation) =>
            validation.Match<IActionResult>(
                Succ: t => new OkObjectResult(t),
                Fail: e => new BadRequestObjectResult(e));

        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Validation<ErrorString, T>> validation) =>
            ToActionResult(await validation);

        public static Task<IActionResult> ToActionResultAsync<T>(this Validation<ErrorString, Task<T>> validation)
        {
            return validation.MatchAsync<IActionResult>(async x => new OkObjectResult(await x),
                err => new BadRequestObjectResult(err));
        }

        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Validation<ErrorString, Task<T>>> validation)
        {
            return await ToActionResultAsync(await validation);
        }

        public static Task<IActionResult> ToActionResultAsync(this Task<Validation<ErrorString, Task>> validation) =>
            validation.Bind(async v => await v.MatchAsync<IActionResult>(
                async t =>
                {
                    await t;
                    return new OkResult();
                },
                e => new BadRequestObjectResult(e)));
    }
}