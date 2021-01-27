using System.Threading.Tasks;

namespace Shared.ConsoleApp.ServiceAppMigration.Interfaces
{
    public interface IRelease
    {
        Task UpgradeAsync();
        Task DowngradeAsync();
    }
}