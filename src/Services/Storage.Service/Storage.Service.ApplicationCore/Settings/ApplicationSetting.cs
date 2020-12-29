namespace Storage.Service.ApplicationCore.Settings
{
    public class ApplicationSetting
    {
        public bool IsCluster { get; set; }
        public OidcSettings Oidc { get; set; } 
    }
}