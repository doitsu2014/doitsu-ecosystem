using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Service.IdentityServer.Constants;
using Identity.Service.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Service.IdentityServer
{
    public class ApplicationUserClaimPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApplicationUserClaimPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor
        ) : base(userManager, roleManager, optionsAccessor)
        {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var listAdditionalClaims = new (string key, string value)[] {
                (ApplicationUserClaimConstant.ADDRESS_STREET, user.Street),
                (ApplicationUserClaimConstant.ADDRESS_COUNTRY, user.Country),
                (ApplicationUserClaimConstant.ADDRESS_STATE, user.State),
                (ApplicationUserClaimConstant.ADDRESS_CITY, user.City),
                (ApplicationUserClaimConstant.ADDRESS_ZIP_CODE, user.ZipCode),
            }
            .Where(kv => !string.IsNullOrEmpty(kv.value))
            .Select(kv => new Claim(kv.key, kv.value))
            .ToArray();
            ((ClaimsIdentity)principal.Identity).AddClaims(listAdditionalClaims);
            return principal;
        }

    }
}