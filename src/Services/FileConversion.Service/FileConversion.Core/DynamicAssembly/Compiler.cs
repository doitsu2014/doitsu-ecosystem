using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Optional;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace FileConversion.Core.DynamicAssembly
{
    public class Compiler
    {
        public Option<byte[], string> CompileDynamicAssemblyFromSourceText(string sourceCode)
        {
            return sourceCode.SomeNotNull().WithException("Source code is null")
                .Filter(x => !string.IsNullOrEmpty(x), "Source code is empty")
                .FlatMap(x =>
                {
                    var t = DateTime.Now;
                    using (var peStream = new MemoryStream())
                    {
                        var result = GenerateCode(sourceCode).Emit(peStream);
                        if (!result.Success)
                        {
                            var error = "";
                            var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                            foreach (var diagnostic in failures)
                            {
                                error += $"{diagnostic.Id}: {diagnostic.GetMessage()}\n";
                            }
                            return Option.None<byte[], string>(error);
                        }
                        peStream.Seek(0, SeekOrigin.Begin);
                        return Option.Some<byte[], string>(peStream.ToArray());
                    }
                });
        }

        private CSharpCompilation GenerateCode(string sourceCode, string assemblyName = "_dynamic.dll")
        {
            var codeString = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8);
            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location),
                MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regex).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location),
                MetadataReference.CreateFromFile(Assembly.Load("Optional").Location),
                MetadataReference.CreateFromFile(Assembly.Load("FileConversion.Core.Interface").Location),
                MetadataReference.CreateFromFile(Assembly.Load("FileConversion.Abstraction").Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
                
            };

            return CSharpCompilation.Create(assemblyName,
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }
    }
}
