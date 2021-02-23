using FileConversion.Core.DynamicAssembly;
using FileConversion.Core.Interface;
using Optional;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileConversion.Core.Services
{
    public class DynamicBeanMapperService : IDynamicBeanMapperService
    {
        public Option<IEnumerable<object>, string> MapFromSource(IEnumerable<object> data, string sourceCode)
            => (data, sourceCode, compiler: new Compiler())
                .SomeNotNull()
                .WithException(string.Empty)
                .Filter(d => d.data != null, "Mapping Data is null, please try again.")
                .Filter(d => !string.IsNullOrEmpty(d.sourceCode), "Source code is null or empty, please try again.")
                .FlatMap(d =>
                {
                    var compiled = d.compiler.CompileDynamicAssemblyFromSourceText(d.sourceCode);
                    return compiled.FlatMap(bytes =>
                    {
                        using (var asm = new MemoryStream(bytes))
                        {
                            var assemblyLoadContext = new UnloadableAssemblyLoadContext();
                            var assembly = assemblyLoadContext.LoadFromStream(asm);
                            var dynamicMapper = assembly
                                .GetTypes()
                                .FirstOrDefault(t => typeof(IBeanMapper).IsAssignableFrom(t) && !t.IsAbstract);
                            var instance = (IBeanMapper)Activator.CreateInstance(dynamicMapper);

                            var result = instance.Map(d.data);

                            assemblyLoadContext.Unload();
                            var assemblyLoadContextWeakRef = new WeakReference(assemblyLoadContext);
                            for (var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++)
                            {
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                            }
                            return result;
                        }
                    });
                });
    }
}
