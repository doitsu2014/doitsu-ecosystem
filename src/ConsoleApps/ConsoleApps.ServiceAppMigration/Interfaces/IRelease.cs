using System.Threading.Tasks;

namespace ConsoleApps.ServiceAppMigration.Interfaces
{
    public interface IRelease
    {
        Task UpgradeAsync();
        Task DowngradeAsync();
    }
}