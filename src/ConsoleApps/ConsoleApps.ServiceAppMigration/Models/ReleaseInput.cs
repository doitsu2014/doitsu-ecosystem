namespace ConsoleApps.ServiceAppMigration.Models
{
    
    public class ReleaseInput
    {
        public ReleaseInputDbConnection DbConnection { get; set; }
        public string AzureKeyVaultConnection { get; set; }
    }

    public class ReleaseInputDbConnection
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}