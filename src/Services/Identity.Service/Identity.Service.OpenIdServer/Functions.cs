using Identity.Service.OpenIdServer.Constants;
using LanguageExt;
using OpenIddict.Abstractions;
using Shared.LanguageExt.Models.Types;
using static LanguageExt.Prelude;

namespace Identity.Service.OpenIdServer
{
    public static class Functions
    {
        public static Validation<ErrorString, string> GetPermissionPrefix(PermissionPrefixEnums value) => value switch
        {
            PermissionPrefixEnums.Endpoint => Success<ErrorString, string>(OpenIddictConstants.Permissions.Prefixes.Endpoint),
            PermissionPrefixEnums.Scope => Success<ErrorString, string>(OpenIddictConstants.Permissions.Prefixes.Scope),
            PermissionPrefixEnums.GrantType => Success<ErrorString, string>(OpenIddictConstants.Permissions.Prefixes.GrantType),
            PermissionPrefixEnums.ResponseType => Success<ErrorString, string>(OpenIddictConstants.Permissions.Prefixes.ResponseType),
            _ => Fail<ErrorString, string>($"Permission prefix {value} does not exist.")
        };
    }
}