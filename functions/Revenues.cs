
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZeeReportingApi.Data;
using System.Collections.Generic;
using System.Data;

namespace ZeeReportingApi

{
    public class Revenues : AuthorizedServiceBase
    {
        [FunctionName("Revenues")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "revenues")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Revenues called");

            var auth = new AuthenticationInfo(req);

            var rankings = DataUtility.CallSproc("[reports].[HomePageRevenues]", auth.Username, useDate: false);

            return new JsonResult(rankings);
        }
    }
}
