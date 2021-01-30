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
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using Shared.Abstraction.Models.Types;
using Shared.Extensions;
using static Shared.Validations.StringValidator;
using static Shared.Validations.GenericValidator;
using static LanguageExt.Prelude;

namespace Identity.Service.OpenIdServer.Controllers
{
    [Authorize(Policy = OidcConstants.PolicyIdentityResourceAll)]
    public class ResourceController : Controller
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
            _oidApplicationManager = oidApplicationManager;
            _oidScopeManager = oidScopeManager;
            _oidTokenManger = oidTokenManger;
            _oidAuthorizationManager = oidAuthorizationManager;
            _mapper = mapper;
        }

        [HttpGet("~/api/resource/application")]
        public async Task<IActionResult> GetApplications()
        {
            return ShouldNotNullOrEmpty(await _oidApplicationManager.ListAsync().ToListAsync())
                .Match<IActionResult>(res =>
                    Ok(res.Map(x => _mapper.Map<OpenIddictApplicationViewModel>(x))), _ => NotFound());
        }

        [HttpGet("~/api/resource/application/{clientId}")]
        public async Task<IActionResult> GetApplications([FromRoute] string clientId)
        {
            return (await ShouldNotNullOrEmpty(clientId).ToEither().MapAsync(async req => ShouldNotNull(await _oidApplicationManager.FindByClientIdAsync(req))))
                .Match<IActionResult>(res => res
                        .Match<IActionResult>(data => Ok(_mapper.Map<OpenIddictApplicationViewModel>(data)),
                            _ => NotFound()),
                    errors => BadRequest(errors.ComposeStrings(", ")));
        }

        [HttpPost("~/api/resource/application/{clientId}/permissions")]
        public async Task<IActionResult> PostPermission([FromRoute] string aid, [FromBody] (PermissionPrefixEnums prefix, string name) req)
        {

            var getExistApplicationById = fun(async (string appId) => await ShouldNotNullOrEmpty(appId)
                .MatchAsync(async d => Optional(await _oidApplicationManager.FindByClientIdAsync(appId))
                        .ToValidation<Error>($"application {d} does not exist."),
                    errors => Fail<Error, OpenIddictEntityFrameworkCoreApplication>(errors.Join())));

            return (await getExistApplicationById(aid), ShouldNotNullOrEmpty(req.name), Success<Error, PermissionPrefixEnums>(req.prefix))
                .Apply((application, permName, permPrefix) => (application, permValue: $"{permPrefix}{permName}"))
                .Map(res => res.application)
                .ToActionResult();
            
            // .Apply((applicationId, permName, permPrefix) => (applicationId, permValue: $"{permPrefix}{permName}"))
            // .ToEither();
            // .MatchAsync<IActionResult>(async t =>
            // {
            //     return Optional(await _oidApplicationManager.FindByClientIdAsync(t.applicationId))
            //         .ToEither("Application does not exist.")
            //         .Match<IActionResult>(app =>
            //         {
            //             var existedPermValue = app.Permissions?.Contains(t.permValue) ?? false;
            //             return Ok(app);
            //         }, BadRequest);
            // }, errors => BadRequest(errors.ComposeStrings(", ")))
            // );

            // .MapAsync(async data => (application: await _oidApplicationManager.FindByClientIdAsync(data.clientId), data.prefix, data.nameof)))
            // .Match<IActionResult>(res => { return Ok(res.application); }, errors => BadRequest(errors.ComposeStrings(", ")));
        }
    }
}