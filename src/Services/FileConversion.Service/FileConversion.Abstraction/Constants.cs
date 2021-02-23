namespace FileConversion.Abstraction
{
    public class Constants
    {
        public const string EnvironmentKeyVaultOption = "ENV_KEY_VAULT";
        public const string EnvironmentSecurityOption = "ENV_SECURITY";
        public const string ConnectionStringKey = "FileConversion";

        public const string DefaultImportStream = "import";
        public const string DefaultExportStream = "export";
        public const string DefaultExportHeaderRecord = "header#";
        public const string DefaultExportFooterRecord = "footer#";
        public const string DefaultExportFileName = "export.txt";
    }

    public static class Security
    {
        public const string Scheme = "conversion";

        public const string ScopeFileConversion = "conversion";

        public const string PolicyFileConversion = "conversion";
    }
}
