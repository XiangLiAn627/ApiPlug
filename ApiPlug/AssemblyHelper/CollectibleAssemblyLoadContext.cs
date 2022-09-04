using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace ApiPlug
{
    /// <summary>
    /// 可回收的程序集加载上下文，我们首先基于AssemblyLoadcontext创建一个CollectibleAssemblyLoadContext类。
    /// 其中我们将IsCollectible属性通过父类构造函数，将其设置为true
    /// 主要用于加载程序集
    /// </summary>
    public class CollectibleAssemblyLoadContext
        : AssemblyLoadContext
    {
        /// <summary>
        /// 将IsCollectible属性设置为true
        /// </summary>
        public CollectibleAssemblyLoadContext()
            : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName name)
        {
            return null;
        }
    }
}
