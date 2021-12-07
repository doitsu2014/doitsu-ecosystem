using System.Threading.Tasks;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;

namespace Shared.LanguageExt.ActionResults
{
    public static class OptionToActionResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Option<T> option) =>
            option.Match<IActionResult>(
                Some: t => new OkObjectResult(t),
                None: () => new NotFoundResult());

        public static Task<IActionResult> ToActionResultAsync<T>(this Task<Option<T>> option) =>
            option.Map(ToActionResult);
    }
}