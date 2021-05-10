
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZeeReportingApi.Data;
using ZeeReportingApi.Model;
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
            log.LogInformation("Get Metrics called");

            var auth = new AuthenticationInfo(req);

            var metrics = DataUtility.CallSproc<string>("reports.dashboardMetrics", "@LoggedInUserEmail", auth.Username, SqlDbType.NVarChar);

            return new JsonResult(metrics);
        }
    }
}
