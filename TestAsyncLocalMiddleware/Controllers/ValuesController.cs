using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.AspNetCore.Mvc;
using TestAsyncLocalMiddleware.Infrastructure;

namespace TestAsyncLocalMiddleware.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DefaultServiceContext _serviceContext;
        private readonly ILog _log;

        public ValuesController(DefaultServiceContext serviceContext, ILog log)
        {
            this._serviceContext = serviceContext;
            this._log = log;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            _log.Info("On Entering Get");
            return new string[] { $"CorpId:{_serviceContext.CorpId}", $"UserId:{_serviceContext.UserId}" };
        }

    }
}
