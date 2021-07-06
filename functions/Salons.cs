
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
    public class Salons : AuthorizedServiceBase
    {
        SalonsRepo _salonsRepo;

        public Salons(ODSContext context)
        {
            _salonsRepo = new SalonsRepo(context);
        }

        [FunctionName("Salons")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "salons")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Get Salons called");

            int franchiseId = 0;

            try
            {
                franchiseId = int.Parse(req.Query["franchiseId"]);
                log.LogInformation("For franchiseId: " + franchiseId.ToString());
            }
            catch (Exception ex)
            {
                return new BadRequestResult();
            }

            var salons = _salonsRepo.GetSalons(franchiseId);

            return new JsonResult(salons);
        }

        [FunctionName("SalonsWithGroups")]
        public async Task<IActionResult> GetSalonsWithGroups(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "salonsWithGroups")] HttpRequest req,
    ILogger log)
        {
            log.LogInformation("Get Salons With Groups called");

            var auth = new AuthenticationInfo(req);

            var salonGroups = _salonsRepo.GetSalonsWithGroups(auth.Username);

            return new JsonResult(new { salonGroups });
        }
    }
}
