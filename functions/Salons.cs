
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzFunctionsJwtAuth;
using ZeeReportingApi.Data;

namespace ZeeReportingApi

{
    public class Salons : AuthorizedServiceBase
    {
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

            var salons = SalonsRepo.GetSalons(franchiseId, log);

            return new JsonResult(salons);
        }
    }
}
