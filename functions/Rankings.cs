
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZeeReportingApi.Data;
using ZeeReportingApi.Model;

namespace ZeeReportingApi

{
    public class Rankings : AuthorizedServiceBase
    {
        RankingsRepo _rankingsRepo;

        public Rankings(ODSContext context)
        {
            _rankingsRepo = new RankingsRepo(context);
        }

        [FunctionName("Rankings")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rankings")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Rankings called");

            var auth = new AuthenticationInfo(req);

            var rankings = _rankingsRepo.GetRankings(auth.Username);

            return new JsonResult(rankings);
        }
    }
}
