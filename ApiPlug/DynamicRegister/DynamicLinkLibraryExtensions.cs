using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace ApiPlug
{
    public class DynamicLinkLibraryExtensions
    {
        public static EmitResult GenerateDynamicLinkLibrary(List<string> codes,string DynamicLinkLibraryName)
        {
            List<MetadataReference> References = new List<MetadataReference>();
            References.Add(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location));
            References.Add(MetadataReference.CreateFromFile(typeof(ControllerBase).GetTypeInfo().Assembly.Location));
            References.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location));
            References.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc")).Location));
            //References.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Newtonsoft.Json")).Location));
            References.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Threading")).Location));
            References.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System")).Location));
            References.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.IO")).Location));
            References.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Reflection")).Location));
            References.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location));
            var compilation = CSharpCompilation.Create(DynamicLinkLibraryName)
               .WithOptions(
                 new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
               .AddReferences(References.ToArray());
            foreach (string code in codes)
            {
                var tree = SyntaxFactory.ParseSyntaxTree(code);
                compilation = compilation.AddSyntaxTrees(tree);
            }
            string path = Path.Combine(Directory.GetCurrentDirectory(), "DynamicLinkLibrary/" + DynamicLinkLibraryName+".dll");
            EmitResult compilationResult = compilation.Emit(path);
            return compilationResult;
        }
    }
}
