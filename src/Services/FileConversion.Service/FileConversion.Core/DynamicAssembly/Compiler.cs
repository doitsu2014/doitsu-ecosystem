using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using LanguageExt;
using Shared.Abstraction.Models.Types;
using static Shared.Validations.StringValidator;
using static LanguageExt.Prelude;

namespace FileConversion.Core.DynamicAssembly
{
    public class Compiler
    {
        public Validation<Error, byte[]> CompileDynamicAssemblyFromSourceText(string sourceCode)
        {
            return ShouldNotNullOrEmpty(sourceCode)
                .Match(sc =>
                    {
                        using (var peStream = new MemoryStream())
                        {
                            var result = GenerateCode(sc).Emit(peStream);
                            if (!result.Success)
                            {
                                var error = "";
                                var failures = result.Diagnostics.Where(diagnostic =>
                                    diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                                foreach (var diagnostic in failures)
                                {
                                    error += $"{diagnostic.Id}: {diagnostic.GetMessage()}\n";
                                }

                                return Fail<Error, byte[]>(error);
                            }

                            peStream.Seek(0, SeekOrigin.Begin);
                            return Success<Error, byte[]>(peStream.ToArray());
                        }
                    },
                    errors => errors.Join());
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
                MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location),
                MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regex).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("LanguageExt").Location),
                MetadataReference.CreateFromFile(Assembly.Load("Shared.Abtraction").Location),
                MetadataReference.CreateFromFile(Assembly.Load("Shared.Validations").Location),
                MetadataReference.CreateFromFile(Assembly.Load("Shared.Extensions").Location),
                MetadataReference.CreateFromFile(Assembly.Load("FileConversion.Core.Interface").Location),
                MetadataReference.CreateFromFile(Assembly.Load("FileConversion.Abstraction").Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
            };

            return CSharpCompilation.Create(assemblyName,
                new[] {parsedSyntaxTree},
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }
    }
}