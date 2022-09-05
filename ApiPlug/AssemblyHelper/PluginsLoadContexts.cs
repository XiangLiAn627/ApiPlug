using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPlug
{
    /// <summary>
    /// 用于操作动态链接库
    /// </summary>
    public static class PluginsLoadContexts
    {
        private static Dictionary<string, CollectibleAssemblyLoadContext>? _pluginContexts = null;

        static PluginsLoadContexts()
        {
            _pluginContexts = new Dictionary<string, CollectibleAssemblyLoadContext>();
        }
        /// <summary>
        /// 判断是否在动态链接库中
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public static bool Any(string pluginName)
        {
            return _pluginContexts!.ContainsKey(pluginName);
        }
        /// <summary>
        /// 移除动态链接库
        /// </summary>
        /// <param name="pluginName"></param>
        public static void RemovePluginContext(string pluginName)
        {
            if (_pluginContexts!.ContainsKey(pluginName))
            {
                _pluginContexts[pluginName].Unload();
                _pluginContexts.Remove(pluginName);
            }
        }
        /// <summary>
        /// 获取动态链接库
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public static CollectibleAssemblyLoadContext GetContext(string pluginName)
        {
            return _pluginContexts![pluginName];
        }
        /// <summary>
        /// 添加动态链接库
        /// </summary>
        /// <param name="pluginName"></param>
        /// <param name="context"></param>
        public static void AddPluginContext(string pluginName,
             CollectibleAssemblyLoadContext context)
        {
            _pluginContexts!.Add(pluginName, context);
        }
    }
}
