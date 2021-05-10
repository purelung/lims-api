
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
    public class SalonRankings : AuthorizedServiceBase
    {
        [FunctionName("SalonRankings")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "salon-rankings")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Salon Rankings called");

            var auth = new AuthenticationInfo(req);

            var rankings = DataUtility.CallSproc<string>("[reports].[salonranking]", "@LoggedInUserEmail", auth.Username, SqlDbType.NVarChar);

            return new JsonResult(rankings);
        }
    }
}