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
    public class NavigateController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Навигация");

            sb.AppendLine();
            sb.AppendLine(@$"<a href=/api/RRLog>Лог запросов</a>");

            return sb.ToString();
        }
    }
}
