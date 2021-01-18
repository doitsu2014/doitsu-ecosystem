using System.Threading.Tasks;
using AutoMapper;
using Identity.Service.OpenIdServer.Constants;
using Identity.Service.OpenIdServer.Helpers;
using Identity.Service.OpenIdServer.Models;
using Identity.Service.OpenIdServer.ViewModels.Resource;
using LanguageExt;
using LanguageExt.SomeHelp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

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
            return (await _oidApplicationManager.ListAsync().ToListAsync())
                .ToOption()
                .Filter(d => d != null)
                .Map(oidApplication => _mapper.Map<OpenIddictApplicationViewModel>(oidApplication))
                .Match<IActionResult>(Ok, NotFound);
        }

        [HttpGet("~/api/resource/application/{clientId}")]
        public async Task<IActionResult> GetApplications([FromRoute] string clientId)
        {
            return (await _oidApplicationManager.FindByClientIdAsync(clientId))
                .ToSome()
                .ToOption()
                .Filter(d => d != null)
                .Map(oApplication => _mapper.Map<OpenIddictApplicationViewModel>(oApplication))
                .Match<IActionResult>(Ok, NotFound);
        }

        [HttpPost("~/api/resource/application/{clientId}/permissions")]
        public async Task<IActionResult> PostPermission([FromRoute] string clientId, [FromBody] (string prefix, string name) request)
        {
            return Optional((clientId, request))
                .ToEither<string, (string, (string, string))>(req =>
                {
                    return None;
                })
                .Filter(d => !string.IsNullOrEmpty(d.prefix))
                .Map(oApplication => _mapper.Map<OpenIddictApplicationViewModel>(oApplication))
                .Match<IActionResult>(Ok, NotFound);
        }
    }
}