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
        public static void GenerateDynamicLinkLibrary(string code,string DynamicLinkLibraryName)
        {
            //code = "using System.Threading.Tasks;using EPN.Core.Service;using EPN.EFCore.Models;using Microsoft.AspNetCore.Mvc;using Newtonsoft.Json.Linq;namespace EPN.Api.Controllers{    [Route(\"api/[controller]\")]    [ApiController]    public class InvokingController : ControllerBase    {        private readonly WorkFlowService Service = new WorkFlowService();        [HttpGet(\"GetWorkFlowWaiteCount\")]        public async Task<string> GetWorkFlowWaiteCount(string UserCode)        {            return await Service.GetWorkFlowWaiteCountByUserCode(UserCode);        }        [HttpPost(\"test1\")]        public async Task<Message<string>> test1(object dynamic)        {            JObject dyn = JObject.Parse(dynamic.ToString());            Message<string> msg = new Message<string>();            msg.Code = 200;            msg.Msg = \"调用接口成功\";            return msg;        }    }}";
            var tree = SyntaxFactory.ParseSyntaxTree(code);
            // Detect the file location for the library that defines the object type
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
            //var systemRefLocation = typeof(object).GetTypeInfo().Assembly.Location;
            //// Create a reference to the library
            //var systemReference = MetadataReference.CreateFromFile(systemRefLocation);
            // A single, immutable invocation to the compiler
            // to produce a library
            var compilation = CSharpCompilation.Create(DynamicLinkLibraryName)
              .WithOptions(
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
              .AddReferences(References.ToArray())
              .AddSyntaxTrees(tree);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "DynamicLinkLibrary/" + DynamicLinkLibraryName+".dll");
            EmitResult compilationResult = compilation.Emit(path);
            if (compilationResult.Success)
            {
                // Load the assembly
            }
            else
            {

            }
        }
    }
}
