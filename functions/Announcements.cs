
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
    public class Announcements : AuthorizedServiceBase
    {
        [FunctionName("Announcements")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "announcements")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Announcements called");

            var auth = new AuthenticationInfo(req);

            var rankings = DataUtility.CallSproc("[app].[sel_Announcemnts]", new List<SprocParam>());

            return new JsonResult(rankings);
        }
    }
}
