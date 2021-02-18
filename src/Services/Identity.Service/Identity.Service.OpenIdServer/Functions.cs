using Identity.Service.OpenIdServer.Constants;
using LanguageExt;
using OpenIddict.Abstractions;
using Shared.Abstraction.Models.Types;
using static LanguageExt.Prelude;

namespace Identity.Service.OpenIdServer
{
    public static class Functions
    {
        public static Validation<Error, string> GetPermissionPrefix(PermissionPrefixEnums value) => value switch
        {
            PermissionPrefixEnums.Endpoint => Success<Error, string>(OpenIddictConstants.Permissions.Prefixes.Endpoint),
            PermissionPrefixEnums.Scope => Success<Error, string>(OpenIddictConstants.Permissions.Prefixes.Scope),
            PermissionPrefixEnums.GrantType => Success<Error, string>(OpenIddictConstants.Permissions.Prefixes.GrantType),
            PermissionPrefixEnums.ResponseType => Success<Error, string>(OpenIddictConstants.Permissions.Prefixes.ResponseType),
            _ => Fail<Error, string>($"Permission prefix {value} does not exist.")
        };
    }
}