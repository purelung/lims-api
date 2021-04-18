
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
    public class Schedules : AuthorizedServiceBase
    {
        [FunctionName("Schedules")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "schedules")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Schedules called");

            var auth = new AuthenticationInfo(req);

            var rankings = DataUtility.CallSproc<string>("[reports].[schedule]", "@LoggedInUserEmail", auth.Username, SqlDbType.NVarChar);

            return new JsonResult(rankings);
        }
    }
}