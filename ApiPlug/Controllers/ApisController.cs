using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ApiPlug.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApisController : ControllerBase
    {

        ApplicationPartManager _partManager;
        public ApisController( ApplicationPartManager partManager)
        {
            _partManager = partManager;
        }
        /// <summary>
        /// 生成动态链接库
        /// </summary>
        /// <param name="code">动态链接库代码</param>
        /// <param name="name">动态链接库名称</param>
        [HttpGet("GenerateDynamicLinkLibrary")]
        public void GenerateDynamicLinkLibrary(string code, string name)
        {
            //code就用TestOneController的代码做测试,name就用TestOne,控制器名不要和其他的控制器名重复
            DynamicLinkLibraryExtensions.GenerateDynamicLinkLibrary(code, name);
        }
        /// <summary>
        /// 删除动态链接库
        /// </summary>
        /// <param name="name">动态链接库名称</param>
        /// <returns></returns>
        [HttpGet("RemovePluginContext")]
        public string RemovePluginContext(string name)
        {
            if (PluginsLoadContexts.Any(name + ".dll"))
            {
                //先操作ApplicationParts动态链接库
                var matchedItem = _partManager.ApplicationParts.FirstOrDefault(p => p.Name == name);
                _partManager.ApplicationParts.Remove(matchedItem);
                //移除数组中的动态链接库
                PluginsLoadContexts.RemovePluginContext(name + ".dll");
                //更新Controllers
                ActionDescriptorChangeProvider.Instance.HasChanged = true;
                ActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                return "移除动态链接库成功";
            }
            else
            {
                return "未找到动态链接库";
            }
        }
        /// <summary>
        /// 新增动态链接库
        /// </summary>
        /// <param name="name">动态链接库名称</param>
        /// <returns></returns>
        [HttpGet("AddPluginContext")]
        public string AddPluginContext(string name)
        {
            if (!PluginsLoadContexts.Any(name + ".dll"))
            {
                //先操作ApplicationParts动态链接库
                //读取动态链接库
                var context = new CollectibleAssemblyLoadContext();
                FileInfo FileInfo = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "DynamicLinkLibrary/" + name + ".dll"));
                var assembly = context.LoadFromStream(new FileStream(FileInfo.FullName, FileMode.Open));
                var controllerAssemblyPart = new AssemblyPart(assembly);
                //插入动态链接库
                _partManager.ApplicationParts.Add(controllerAssemblyPart);
                //插入数组
                PluginsLoadContexts.AddPluginContext(name + ".dll", context);
                //更新Controllers
                ActionDescriptorChangeProvider.Instance.HasChanged = true;
                ActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                return "添加动态链接库成功";
            }
            else
            {
                return "动态链接库已存在";
            }
        }

    }
}