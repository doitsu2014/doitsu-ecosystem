using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Service.OpenIdServer.Constants;
using Identity.Service.OpenIdServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.Service.OpenIdServer.Custom
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
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
                (ClaimTypeConstants.Avatar, user.Avatar),
                (ClaimTypeConstants.AddressStreet, user.Street),
                (ClaimTypeConstants.AddressCountry, user.Country),
                (ClaimTypeConstants.AddressState, user.State),
                (ClaimTypeConstants.AddressCity, user.City),
                (ClaimTypeConstants.AddressZipCode, user.ZipCode)
            }
            .Where(kv => !string.IsNullOrEmpty(kv.value))
            .Select(kv => new Claim(kv.key, kv.value))
            .ToArray();
            ((ClaimsIdentity)principal.Identity).AddClaims(listAdditionalClaims);
            return principal;
        }

    }
}