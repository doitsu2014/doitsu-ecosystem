using Identity.Service.IdentityServer.Constants;
using IdentityModel;
using IdentityServer4.Models;

namespace Identity.Service.IdentityServer.Profiles
{
    public class AddressIdentityResource : IdentityResource
    {
        public AddressIdentityResource()
        {
            this.Name = "address";
            this.DisplayName = "Address Identity Resource";
            this.UserClaims = new string[]
            {
                ApplicationUserClaimConstant.ADDRESS_CITY,
                ApplicationUserClaimConstant.ADDRESS_COUNTRY,
                ApplicationUserClaimConstant.ADDRESS_STATE,
                ApplicationUserClaimConstant.ADDRESS_STREET,
                ApplicationUserClaimConstant.ADDRESS_ZIP_CODE
            };
        }
    }
}