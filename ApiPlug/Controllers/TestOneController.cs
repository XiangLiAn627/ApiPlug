using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EPN.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestOneController : ControllerBase
    {
        [HttpGet("Test")]
        public string Test(string str)
        {
            return "Test:" + str;
        }
    }
}
