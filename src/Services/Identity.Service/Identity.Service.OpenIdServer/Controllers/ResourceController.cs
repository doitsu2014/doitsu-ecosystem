using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Service.OpenIdServer.Constants;
using Identity.Service.OpenIdServer.Helpers;
using Identity.Service.OpenIdServer.Models;
using Identity.Service.OpenIdServer.ViewModels.Resource;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using Shared.LanguageExt.ActionResults;
using Shared.LanguageExt.Common;
using Shared.LanguageExt.Models.Types;
using Shared.LanguageExt.Validators;
using static LanguageExt.Prelude;

namespace Identity.Service.OpenIdServer.Controllers
{
    [Authorize(Policy = OidcConstants.PolicyIdentityResourceAll)]
    public class ResourceController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> _oidApplicationManager;
        private readonly IOpenIddictScopeManager _oidScopeManager;
        private readonly IOpenIddictTokenManager _oidTokenManger;
        private readonly IOpenIddictAuthorizationManager _oidAuthorizationManager;
        private readonly IMapper _mapper;

        public ResourceController(UserManager<ApplicationUser> userManager,
            OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> oidApplicationManager,
            IOpenIddictScopeManager oidScopeManager,
            IOpenIddictTokenManager oidTokenManger,
            IOpenIddictAuthorizationManager oidAuthorizationManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _oidScopeManager = oidScopeManager;
            _oidTokenManger = oidTokenManger;
            _oidAuthorizationManager = oidAuthorizationManager;
            _mapper = mapper;
            _oidApplicationManager = oidApplicationManager;
        }

        [HttpGet("~/api/resource/application")]
        public async Task<IActionResult> GetApplications()
        {
            return (await _oidApplicationManager.ListAsync().ToListAsync())
                .ShouldNotNullOrEmpty()
                .Match<IActionResult>(res =>
                    Ok(res.Map(x => _mapper.Map<OpenIddictApplicationViewModel>(x))), _ => NotFound());
        }

        [HttpGet("~/api/resource/application/{clientId}")]
        public async Task<IActionResult> GetApplications([FromRoute] string clientId)
        {
            return (await clientId.ShouldNotNullOrEmpty("clientId is null or empty").ToEither()
                    .MapAsync(async req => (await _oidApplicationManager.FindByClientIdAsync(req)).ShouldNotNull()))
                .Match(res => res
                        .Match<IActionResult>(data => Ok(_mapper.Map<OpenIddictApplicationViewModel>(data)),
                            _ => NotFound()),
                    errors => BadRequest(errors.Join()));
        }

        private async Task<Validation<ErrorString, OpenIddictEntityFrameworkCoreApplication>> GetExistApplicationById(string appId)
        {
            return await appId
                .ShouldNotNullOrEmpty("appId is null or empty")
                .MapAsync(async appId => await _oidApplicationManager
                    .FindByClientIdAsync(appId)
                    .ShouldNotNullAsync($"application {appId} does not exist"));
        }

        [HttpPost("~/api/resource/application/{aid}/permissions")]
        public async Task<IActionResult> PostPermission([FromBody] EditApplicationPermissionViewModel req,
            [FromRoute] string aid)
        {
            var permValueMustUnique = fun((OpenIddictEntityFrameworkCoreApplication app, string permValue) =>
            {
                var listPermissions = JsonConvert.DeserializeObject<System.Collections.Generic.HashSet<string>>(app.Permissions);
                if (listPermissions.Any(x => x == permValue))
                {
                    return Fail<ErrorString, (OpenIddictEntityFrameworkCoreApplication application, string availListP)>(
                        $"Permission {permValue} does exist in application");
                }

                listPermissions.Add(permValue);
                return Success<ErrorString, (OpenIddictEntityFrameworkCoreApplication application, string availListP)>((app,
                    JsonConvert.SerializeObject(listPermissions)));
            });

            return await (await GetExistApplicationById(aid), req.Name.ShouldNotNullOrEmpty(),
                    Functions.GetPermissionPrefix(req.Prefix))
                .Apply((application, permName, permPrefix) => (app: application, permValue: $"{permPrefix}{permName}"))
                .Bind(x => permValueMustUnique(x.app, x.permValue))
                .MatchAsync(async res =>
                {
                    res.application.Permissions = res.availListP;
                    await _oidApplicationManager.UpdateAsync(res.application);
                    return Success<ErrorString, string>(
                        $"Did modify new value {res.availListP} to permission field of application {res.application.DisplayName}");
                }, errors => Fail<ErrorString, string>(errors.Join()))
                .ToActionResultAsync();
        }

        [HttpDelete("~/api/resource/application/{aid}/permissions")]
        public async Task<IActionResult> DeletePermission([FromRoute] string aid,
            [FromBody] EditApplicationPermissionViewModel req)
        {
            var permValueMustExisted = fun((OpenIddictEntityFrameworkCoreApplication app, string permValue) =>
            {
                var listPermissions =
                    JsonConvert.DeserializeObject<System.Collections.Generic.HashSet<string>>(app.Permissions);
                if (listPermissions.Any(x => x == permValue))
                {
                    listPermissions.Remove(permValue);
                    return Success<ErrorString, (OpenIddictEntityFrameworkCoreApplication application, string availListP)>((
                        app, JsonConvert.SerializeObject(listPermissions)));
                }

                return Fail<ErrorString, (OpenIddictEntityFrameworkCoreApplication application, string availListP)>(
                    $"Permission {permValue} does not exist in application");
            });

            return await (await GetExistApplicationById(aid), req.Name.ShouldNotNullOrEmpty(),
                    Functions.GetPermissionPrefix(req.Prefix))
                .Apply((application, permName, permPrefix) => (app: application, permValue: $"{permPrefix}{permName}"))
                .Bind(x => permValueMustExisted(x.app, x.permValue))
                .MatchAsync(async res =>
                {
                    res.application.Permissions = res.availListP;
                    await _oidApplicationManager.UpdateAsync(res.application);
                    return Success<ErrorString, string>(
                        $"Did modify new value {res.availListP} to permission field of application {res.application.DisplayName}");
                }, errors => Fail<ErrorString, string>(errors.Join()))
                .ToActionResultAsync();
        }

        [HttpPost("~/api/resource/application")]
        public async Task<IActionResult> PostApplication([FromBody] OpenIddictApplicationDescriptor descriptor)
        {
            return await (await _oidApplicationManager.FindByClientIdAsync(descriptor.ClientId) == null
                    ? Success<ErrorString, OpenIddictApplicationDescriptor>(descriptor)
                    : Fail<ErrorString, OpenIddictApplicationDescriptor>($"Application {descriptor.ClientId} does exist"))
                .MatchAsync(
                    async req =>
                    {
                        return Success<ErrorString, OpenIddictEntityFrameworkCoreApplication>(
                            await _oidApplicationManager.CreateAsync(req));
                    }, errors => Fail<ErrorString, OpenIddictEntityFrameworkCoreApplication>(errors.Join()))
                .ToActionResultAsync();
        }

        [HttpDelete("~/api/resource/application/{appId}")]
        public async Task<IActionResult> DeleteApplication([FromRoute] string appId)
        {
            return await (await GetExistApplicationById(appId))
                .MatchAsync(async res =>
                {
                    foreach (var token in res.Tokens)
                    {
                        await _oidTokenManger.DeleteAsync(token);
                    }

                    foreach (var token in res.Authorizations)
                    {
                        await _oidAuthorizationManager.DeleteAsync(token);
                    }

                    await _oidApplicationManager.DeleteAsync(res);
                    return Success<ErrorString, string>($"Removed application {res.DisplayName}");
                }, errors => Fail<ErrorString, string>(errors.Join()))
                .ToActionResultAsync();
        }
    }
}