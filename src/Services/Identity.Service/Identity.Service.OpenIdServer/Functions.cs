using Identity.Service.OpenIdServer.Constants;
using LanguageExt;
using OpenIddict.Abstractions;
using static LanguageExt.Prelude;

namespace Identity.Service.OpenIdServer
{
    public static class Functions
    {
        public static Validation<string, string> GetPermissionPrefix(PermissionPrefixEnums value) => value switch
        {
            PermissionPrefixEnums.Endpoint => Success<string, string>(OpenIddictConstants.Permissions.Prefixes.Endpoint),
            PermissionPrefixEnums.Scope => Success<string, string>(OpenIddictConstants.Permissions.Prefixes.Scope),
            PermissionPrefixEnums.GrantType => Success<string, string>(OpenIddictConstants.Permissions.Prefixes.GrantType),
            PermissionPrefixEnums.ResponseType => Success<string, string>(OpenIddictConstants.Permissions.Prefixes.ResponseType),
            _ => Fail<string, string>($"Permission Prefix {value} does not exist.")
        };
    }
}