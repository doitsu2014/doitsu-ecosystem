using FileConversion.Core.DynamicAssembly;
using FileConversion.Core.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LanguageExt;
using Shared.Abstraction.Models.Types;
using static LanguageExt.Prelude;
using static Shared.Validations.GenericValidator;
using static Shared.Validations.StringValidator;

namespace FileConversion.Core.Services
{
    public class DynamicBeanMapperService : IDynamicBeanMapperService
    {
        public Either<Error, IEnumerable<object>> MapFromSource(IEnumerable<object> destinationData, string sourceCode)
            => (ShouldNotNull(destinationData), ShouldNotNullOrEmpty(sourceCode), Success<Error, Compiler>(new Compiler()))
                .Apply((des, sc, compiler) =>
                {
                    return compiler.CompileDynamicAssemblyFromSourceText(sc)
                        .Match((compiled) =>
                        {
                            using (var asm = new MemoryStream(compiled))
                            {
                                var assemblyLoadContext = new UnloadableAssemblyLoadContext();
                                var assembly = assemblyLoadContext.LoadFromStream(asm);
                                var dynamicMapper = assembly
                                    .GetTypes()
                                    .FirstOrDefault(t => typeof(IBeanMapper).IsAssignableFrom(t) && !t.IsAbstract);
                                var instance = (IBeanMapper) Activator.CreateInstance(dynamicMapper);
                                var result = instance.Map(des);
                                assemblyLoadContext.Unload();
                                var assemblyLoadContextWeakRef = new WeakReference(assemblyLoadContext);
                                for (var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++)
                                {
                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();
                                }

                                return result;
                            }
                        }, errors => errors.Join());
                });
    }
}