
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZeeReportingApi.Data;
using System.Data;
using System.Collections.Generic;

namespace ZeeReportingApi

{
    public class dashEmployees : AuthorizedServiceBase
    {
        [FunctionName("dashboardEmployees")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dashboardEmployees")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Dashboard Employees called");

            var auth = new AuthenticationInfo(req);

            var dashEployees = DataUtility.CallSproc("reports.dashboardEmployees", auth.Username);

            return new JsonResult(dashEployees);
        }
    }
}
