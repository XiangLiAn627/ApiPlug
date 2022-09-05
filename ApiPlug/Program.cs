using ApiPlug;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region 插件化开发
//依赖注入监听动态连接池变化
builder.Services.AddSingleton<IActionDescriptorChangeProvider>(ActionDescriptorChangeProvider.Instance);
builder.Services.AddSingleton(ActionDescriptorChangeProvider.Instance);
//启动载入所有动态连接池
var mvcBuilders = builder.Services.AddMvc();
mvcBuilders.ConfigureApplicationPartManager(apm =>
{
    var context = new CollectibleAssemblyLoadContext();
    DirectoryInfo DirInfo = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "DynamicLinkLibrary"));
    FileInfo[] DynamicLinkLibrarys = DirInfo.GetFiles();
    foreach (FileInfo DynamicLinkLibrary in DynamicLinkLibrarys)
    {
        using (FileStream fs = new FileStream(DynamicLinkLibrary.FullName, FileMode.Open))
        {
            var assembly = context.LoadFromStream(fs);
            //var assembly = Assembly.LoadFile(DynamicLinkLibrary.FullName);
            var controllerAssemblyPart = new AssemblyPart(assembly);
            apm.ApplicationParts.Add(controllerAssemblyPart);
            PluginsLoadContexts.AddPluginContext(DynamicLinkLibrary.Name, context);
        }
    }
});

mvcBuilders.SetCompatibilityVersion(CompatibilityVersion.Latest);
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
