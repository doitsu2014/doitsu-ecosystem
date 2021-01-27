using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shared.ConsoleApp.Extension;

namespace Shared.ConsoleApp.ServiceAppMigration
{
    public class Program
    {
        public static async Task Main(string[] args) => await ServiceProviderFactory.GetServiceProvider(
                addCustomServices: (sc, conf) =>
                {
                },
                jsonFilePath: "Configurations/appSettings.json",
                assembly: Assembly.GetAssembly(typeof(Program))
            ).MatchAsync(
                async sp =>
                {
                    using var scope = sp.CreateScope();
                    return true;
                }, () => false);
    }
}