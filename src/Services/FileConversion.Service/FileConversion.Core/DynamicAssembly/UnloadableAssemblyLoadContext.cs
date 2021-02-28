using System.Reflection;
using System.Runtime.Loader;

namespace FileConversion.Core.DynamicAssembly
{
    public class UnloadableAssemblyLoadContext : AssemblyLoadContext
    {
        public UnloadableAssemblyLoadContext() : base(true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
