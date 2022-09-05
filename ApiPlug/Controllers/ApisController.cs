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
            List<string> codes = new List<string>();
            codes.Add(code);
            DynamicLinkLibraryExtensions.GenerateDynamicLinkLibrary(codes, name);
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
                if (matchedItem != null)
                {
                    _partManager.ApplicationParts.Remove(matchedItem);
                    matchedItem = null;
                }
                //����Controllers
                ActionDescriptorChangeProvider.Instance.HasChanged = true;
                ActionDescriptorChangeProvider.Instance.TokenSource!.Cancel();
                //�Ƴ������еĶ�̬���ӿ�
                PluginsLoadContexts.RemovePluginContext(name + ".dll");
                //GC.Collect();�������ʱ������using�Ͳ���Ҫʹ��GC��
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
                using (FileStream fs = new FileStream(FileInfo.FullName, FileMode.Open))
                {
                    var assembly = context.LoadFromStream(fs);
                    var controllerAssemblyPart = new AssemblyPart(assembly);
                    //���붯̬���ӿ�
                    _partManager.ApplicationParts.Add(controllerAssemblyPart);
                    //��������
                    PluginsLoadContexts.AddPluginContext(name + ".dll", context);
                    //����Controllers
                    ActionDescriptorChangeProvider.Instance.HasChanged = true;
                    ActionDescriptorChangeProvider.Instance.TokenSource!.Cancel();
                }
                return "��Ӷ�̬���ӿ�ɹ�";
            }
            else
            {
                return "��̬���ӿ��Ѵ���";
            }
        }

    }
}