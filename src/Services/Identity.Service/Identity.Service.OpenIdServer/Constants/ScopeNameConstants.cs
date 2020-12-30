using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Service.OpenIdServer.Constants
{
    public static class ScopeNameConstants
    {
        public const string ScopeBlogPostRead = "services.blogpost.read";
        public const string ScopeBlogPostWrite = "services.blogpost.write";
        
        public const string ScopeImageServerRead = "services.imageserver.read";
        public const string ScopeImageServerWrite = "services.imageserver.write";
        
        public const string ScopeIdentityServerAllServices = "services.identity.all";
        public const string ScopeIdentityServerUserInfo = "services.identity.userinfo";
    }
}