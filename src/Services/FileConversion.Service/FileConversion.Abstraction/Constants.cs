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

    public static class PolicyConstants
    {
        public const string PolicyFileConversionRead = "policy.read";
        public const string PolicyFileConversionParse = "policy.parse";
        public const string PolicyFileConversionWrite = "policy.write";
        
        public const string ScopeFileConversionAll = "services.fileconversion.all";
        public const string ScopeFileConversionRead = "services.fileconversion.read";
        public const string ScopeFileConversionParse = "services.fileconversion.parse";
        public const string ScopeFileConversionWrite = "services.fileconversion.write";
    }
}