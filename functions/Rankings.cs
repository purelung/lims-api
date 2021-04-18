
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
    public class Rankings : AuthorizedServiceBase
    {
        [FunctionName("Rankings")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rankings")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Rankings called");

            var auth = new AuthenticationInfo(req);

            var rankings = DataUtility.CallSproc<string>("[reports].[ranking]", "@LoggedInUserEmail", auth.Username, SqlDbType.NVarChar);

            return new JsonResult(rankings);
        }
    }
}
