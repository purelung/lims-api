
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
    public class dashPredictive : AuthorizedServiceBase
    {
        [FunctionName("dashboardPredictive")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dashboardPredictive")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Dashboard Schedules called");

            var auth = new AuthenticationInfo(req);

            string storeId = "";

            try
            {
                storeId = req.Query["storeId"];
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }

            var sprocParams = new List<SprocParam>() {
                DataUtility.GetUser(auth.Username),
                DataUtility.GetStoreId(storeId),
            };

            var dashPred = DataUtility.CallSproc("reports.PredictiveSchedule", sprocParams);

            return new JsonResult(dashPred);
        }
    }
}
