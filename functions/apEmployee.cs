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
    public class apEmployee : AuthorizedServiceBase
    {
        [FunctionName("apEmployee")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "apEmployee")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get AP Employee called");

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

            var dashEployees = DataUtility.CallSproc("mgmtReports.assetProtectionEmployee", sprocParams);

            return new JsonResult(dashEployees);
        }
    }
}
