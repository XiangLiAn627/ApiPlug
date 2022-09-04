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
        /// ���ɶ�̬���ӿ�
        /// </summary>
        /// <param name="code">��̬���ӿ����</param>
        /// <param name="name">��̬���ӿ�����</param>
        [HttpGet("GenerateDynamicLinkLibrary")]
        public void GenerateDynamicLinkLibrary(string code, string name)
        {
            //code����TestOneController�Ĵ���������,name����TestOne,����������Ҫ�������Ŀ��������ظ�
            DynamicLinkLibraryExtensions.GenerateDynamicLinkLibrary(code, name);
        }
        /// <summary>
        /// ɾ����̬���ӿ�
        /// </summary>
        /// <param name="name">��̬���ӿ�����</param>
        /// <returns></returns>
        [HttpGet("RemovePluginContext")]
        public string RemovePluginContext(string name)
        {
            if (PluginsLoadContexts.Any(name + ".dll"))
            {
                //�Ȳ���ApplicationParts��̬���ӿ�
                var matchedItem = _partManager.ApplicationParts.FirstOrDefault(p => p.Name == name);
                _partManager.ApplicationParts.Remove(matchedItem);
                //�Ƴ������еĶ�̬���ӿ�
                PluginsLoadContexts.RemovePluginContext(name + ".dll");
                //����Controllers
                ActionDescriptorChangeProvider.Instance.HasChanged = true;
                ActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                return "�Ƴ���̬���ӿ�ɹ�";
            }
            else
            {
                return "δ�ҵ���̬���ӿ�";
            }
        }
        /// <summary>
        /// ������̬���ӿ�
        /// </summary>
        /// <param name="name">��̬���ӿ�����</param>
        /// <returns></returns>
        [HttpGet("AddPluginContext")]
        public string AddPluginContext(string name)
        {
            if (!PluginsLoadContexts.Any(name + ".dll"))
            {
                //�Ȳ���ApplicationParts��̬���ӿ�
                //��ȡ��̬���ӿ�
                var context = new CollectibleAssemblyLoadContext();
                FileInfo FileInfo = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "DynamicLinkLibrary/" + name + ".dll"));
                var assembly = context.LoadFromStream(new FileStream(FileInfo.FullName, FileMode.Open));
                var controllerAssemblyPart = new AssemblyPart(assembly);
                //���붯̬���ӿ�
                _partManager.ApplicationParts.Add(controllerAssemblyPart);
                //��������
                PluginsLoadContexts.AddPluginContext(name + ".dll", context);
                //����Controllers
                ActionDescriptorChangeProvider.Instance.HasChanged = true;
                ActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                return "��Ӷ�̬���ӿ�ɹ�";
            }
            else
            {
                return "��̬���ӿ��Ѵ���";
            }
        }

    }
}