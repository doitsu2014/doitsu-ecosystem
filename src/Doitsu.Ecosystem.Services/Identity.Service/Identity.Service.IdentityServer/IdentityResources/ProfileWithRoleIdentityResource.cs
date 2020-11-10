using IdentityModel;
using static IdentityServer4.Models.IdentityResources;

namespace Identity.Service.IdentityServer.IdentityResources
{
    public class ProfileWithRoleIdentityResource : Profile
    {
        public ProfileWithRoleIdentityResource()
        {
            this.UserClaims.Add(JwtClaimTypes.Role);
        }
    }
}