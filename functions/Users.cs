
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzFunctionsJwtAuth;
using ZeeReportingApi.Data;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace ZeeReportingApi

{
    public class Users : AuthorizedServiceBase
    {
        [FunctionName("Users")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "users")] HttpRequest req,
            ILogger log)
        {
            if (req.Method == HttpMethods.Get)
            {
                log.LogInformation("Get Users called");

                var users = UsersRepo.GetUsers();

                return new JsonResult(users);
            }
            else if (req.Method == HttpMethods.Post)
            {
                log.LogInformation("Add User called");

                var reader = new StreamReader(req.Body, Encoding.UTF8);

                var requestBody = await reader.ReadToEndAsync();

                User user = JsonConvert.DeserializeObject<User>(requestBody);

                var result = UsersRepo.AddUser(user);

                if (result)
                {
                    return new JsonResult(new { status = "success" });
                }
                else
                {
                    return new BadRequestResult();
                }
            }

            return new BadRequestResult();
        }
    }
}
