using System;
using Identity.Service.OpenIdServer.Models.DataTransferObjects;

namespace Identity.Service.OpenIdServer.Settings;

public class AppSetting
{
    public AwsS3Settings AwsS3Settings { get; set; }
    public InitialSetting InitialSetting { get; set; }
}

public class InitialSetting
{
    public InitialApplication[] Applications { get; set; }
    public InitialUser[] Users { get; set; }
    public InitialResource[] Resources { get; set; }

    public class InitialUser : CreateUserWithRolesDto
    {
    }

    public class InitialResource
    {
        public string AudienceName { get; set; }
        public string[] Scopes { get; set; }
    }

    public class InitialApplication
    {
        public string ClientId { get; set; }
        public string DisplayName { get; set; }

        public string ClientSecret { get; set; }

        public string ConsentType { get; set; }

        public string[] Permissions { get; set; }

        public Uri[] PostLogoutRedirectUris { get; set; }

        public Uri[] RedirectUris { get; set; }

        public string[] Requirements { get; set; }

        public string Type { get; set; }
    }
}

public class AwsS3Settings
{
    public string Url { get; set; }
    public string AccessKeyId { get; set; }
    public string AccessKeySecret { get; set; }
    public string BucketName { get; set; }
}