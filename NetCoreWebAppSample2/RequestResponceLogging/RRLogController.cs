using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreWebAppSample2.RequestResponceLogging
{
    [Route("api/[controller]")]
    [ApiController]
    public class RRLogController : ControllerBase
    {
        private IResponceRequestLogger _logger;

        public RRLogController(IResponceRequestLogger logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<ResponceRequestLogItem>  ReadLog()
        {
            return _logger.GetAllMessages();
        }
    }
}
