
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
    public class dashSchedules : AuthorizedServiceBase
    {
        [FunctionName("dashboardSchedules")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dashboardSchedules")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Dashboard Schedules called");

            var auth = new AuthenticationInfo(req);

            var dashSchedules = DataUtility.CallSproc("reports.dashboardSchedules", auth.Username);

            return new JsonResult(dashSchedules);
        }
    }
}
