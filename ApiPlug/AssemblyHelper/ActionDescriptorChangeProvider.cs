using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiPlug
{
    /// <summary>
    /// 用于刷新Controllers，否则操作动态链接库之后没有效果
    /// </summary>
    public class ActionDescriptorChangeProvider : IActionDescriptorChangeProvider
    {
        public static ActionDescriptorChangeProvider Instance { get; } = new ActionDescriptorChangeProvider();

        public CancellationTokenSource? TokenSource { get; private set; }

        public bool HasChanged { get; set; }
        /// <summary>
        /// 监听Token改变
        /// </summary>
        /// <returns></returns>

        public IChangeToken GetChangeToken()
        {
            TokenSource = new CancellationTokenSource();
            return new CancellationChangeToken(TokenSource.Token);
        }
    }
}
