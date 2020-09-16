using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreWebAppSample2.Controllers;

namespace NetCoreWebAppCustom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChildController : ResponceCachingSampleController
    {
    }
}
