namespace Shared.ConsoleApp.ServiceAppMigration.Models
{
    public class ReleaseInput
    {
        public ReleaseInputDbConnectionString ConnectionString { get; set; }
    }

    public class ReleaseInputDbConnectionString
    {
        public string Connection { get; set; }
        public string Host { get; set; }
    }
}