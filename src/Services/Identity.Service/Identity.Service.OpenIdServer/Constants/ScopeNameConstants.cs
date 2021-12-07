using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Service.OpenIdServer.Constants
{
    public static class ScopeNameConstants
    {
        public const string ScopePostsAll = "services.blogpost.all";
        public const string ScopePostsRead = "services.blogpost.read";
        public const string ScopePostsWrite = "services.blogpost.write";
        
        public const string ScopeImageServerAll = "services.imageserver.All";
        public const string ScopeImageServerRead = "services.imageserver.read";
        public const string ScopeImageServerWrite = "services.imageserver.write";
        
        public const string ScopeIdentityServerAll = "services.identity.all";
        public const string ScopeIdentityServerUserInfo = "services.identity.userinfo";
        
        public const string ScopeFileConversionAll = "services.fileconversion.all";
        public const string ScopeFileConversionRead = "services.fileconversion.read";
        public const string ScopeFileConversionParse = "services.fileconversion.parse";
        public const string ScopeFileConversionWrite = "services.fileconversion.write";
    }
}