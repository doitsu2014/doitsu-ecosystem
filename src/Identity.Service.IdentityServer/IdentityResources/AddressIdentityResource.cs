using Identity.Service.IdentityServer.Constants;
using IdentityServer4.Models;

namespace Identity.Service.IdentityServer.Profiles
{
    public class AddressIdentityResource : IdentityResource
    {
        public AddressIdentityResource()
        {
            this.Name = StandardScopeConstants.ADDRESS;
            this.DisplayName = "Address identity resource";
            this.UserClaims = new string[]
            {
                ApplicationUserClaimConstants.ADDRESS_CITY,
                ApplicationUserClaimConstants.ADDRESS_COUNTRY,
                ApplicationUserClaimConstants.ADDRESS_STATE,
                ApplicationUserClaimConstants.ADDRESS_STREET,
                ApplicationUserClaimConstants.ADDRESS_ZIP_CODE
            };
        }
    }
}