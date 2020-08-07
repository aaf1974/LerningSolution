using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetCoreWebAppSample2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NavigateController : ControllerBase
    {
        //https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1
        private readonly ILogger<NavigateController> _logger;
        public NavigateController(ILogger<NavigateController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Index()
        {
            _logger.LogInformation("[GET] [Navigation] called.");


            StringBuilder sb = new StringBuilder();
            sb.Append("Навигация");

            sb.AppendLine();
            sb.AppendLine(@$"<a href=/api/RRLog>Лог запросов</a>");
            sb.AppendLine(@$"<a href=/api/MCache>Лог запросов</a>");



            return sb.ToString();
        }
    }
}
