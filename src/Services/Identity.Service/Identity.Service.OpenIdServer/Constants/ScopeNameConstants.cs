using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Service.OpenIdServer.Constants
{
    public static class ScopeNameConstants
    {
        public const string ScopePostsAll = "services.posts.all";
        public const string ScopePostsRead = "services.posts.read";
        public const string ScopePostsWrite = "services.posts.write";
        
        public const string ScopeImageServerAll = "services.imageserver.all";
        public const string ScopeImageServerRead = "services.imageserver.read";
        public const string ScopeImageServerWrite = "services.imageserver.write";
        
        public const string ScopeIdentityServerAll = "services.identity.all";
        public const string ScopeIdentityServerUserInfo = "services.identity.userinfo";
        
        public const string ScopeFileConversionAll =   "services.fileconversion.all";
        public const string ScopeFileConversionRead =  "services.fileconversion.read";
        public const string ScopeFileConversionParse = "services.fileconversion.parse";
        public const string ScopeFileConversionWrite = "services.fileconversion.write";
    }
}