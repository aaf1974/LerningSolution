using System;
using System.Collections.Generic;
using System.Linq;
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
            return "Навигация";
        }
    }
}
