using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreWebAppSample2.MemoryCacheSample
{
    //https://docs.microsoft.com/ru-ru/aspnet/core/performance/caching/memory?view=aspnetcore-3.1

    [Route("api/[controller]")]
    [ApiController]
    public class MCacheController : ControllerBase
    {
        private MemoryCacheService _cacheService;

        public MCacheController(MemoryCacheService cacheService)
        {
            _cacheService = cacheService;
        }


        [HttpGet]
        public ActionResult<MemoryCacheService.GetSampleDataResponce> GetSample1()
        {
            return _cacheService.GetSampleData();
        }
    }
}
