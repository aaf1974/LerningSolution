using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreWebAppSample2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResponceCachingSampleController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Вызов базового контроллера");

            return sb.ToString();
        }
    }
}
