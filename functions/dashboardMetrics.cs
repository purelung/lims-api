
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

            var startDate = new DateTime();
            var endDate = new DateTime();

            try
            {
                startDate = DateTime.Parse(req.Query["startDate"]);
                endDate = DateTime.Parse(req.Query["endDate"]);
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }

            var sprocParams = new List<SprocParam>() {
                DataUtility.GetUser(auth.Username),
                DataUtility.GetStartDate(startDate),
                DataUtility.GetEndDate(endDate)
            };

            var dashMetrics = DataUtility.CallSproc("reports.dashboardSalonMetrics", sprocParams);

            return new JsonResult(dashMetrics);
        }
    }
}
