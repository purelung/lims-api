
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
    public class Metrics : AuthorizedServiceBase
    {
        [FunctionName("dashboardMetrics")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dashboardMetrics")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Dashboard Metrics called");

            var auth = new AuthenticationInfo(req);

            var dashMetrics = DataUtility.CallSproc("reports.dashboardMetrics", auth.Username);

            return new JsonResult(dashMetrics);
        }
    }
}
