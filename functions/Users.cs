
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZeeReportingApi.Data;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using ZeeReportingApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeeReportingApi

{
    public class Users : AuthorizedServiceBase
    {
        private UsersRepo _usersRepo;
        private SalonsRepo _salonsRepo;

        public Users(ODSContext context)
        {
            _usersRepo = new UsersRepo(context);
            _salonsRepo = new SalonsRepo(context);
        }

        [FunctionName("Users")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", "post", Route = "users")] HttpRequest req,
            ILogger log)
        {
            if (req.Method == HttpMethods.Get && !string.IsNullOrEmpty(req.Query["franchiseId"]))
            {
                log.LogInformation("Get Users called");

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

                var users = _usersRepo.GetUsers(franchiseId);

                return new JsonResult(users);
            }
            else if (req.Method == HttpMethods.Get && !string.IsNullOrEmpty(req.Query["userId"]))
            {
                log.LogInformation("Get User called");

                int userId = 0;

                try
                {
                    userId = int.Parse(req.Query["userId"]);
                    log.LogInformation("For userId: " + userId.ToString());
                }
                catch (Exception ex)
                {
                    return new BadRequestResult();
                }

                var users = _usersRepo.GetUser(userId);

                if (users.Count == 0)
                {
                    return new NotFoundResult();
                }

                var user = users.FirstOrDefault();

                var userWithSalons = _salonsRepo.GetUserWithSalons(user);

                return new JsonResult(userWithSalons);
            }
            else if (req.Method == HttpMethods.Post)
            {
                log.LogInformation("Add User called");

                var reader = new StreamReader(req.Body, Encoding.UTF8);

                var requestBody = await reader.ReadToEndAsync();

                log.LogInformation(requestBody);

                UserWithSalons userWithSalons = JsonConvert.DeserializeObject<UserWithSalons>(requestBody);

                int userId = _usersRepo.AddUser(userWithSalons.user);
                var userSalons = userWithSalons.selectedSalons.Select(s => new UserXSalon() { UserId = userId, SalonId = int.Parse(s.Id) }).ToList();
                _usersRepo.AddUserSalons(userSalons);

                return new JsonResult(new { status = "success" });
            }
            else if (req.Method == HttpMethods.Put)
            {
                log.LogInformation("Update User called");

                var reader = new StreamReader(req.Body, Encoding.UTF8);

                var requestBody = await reader.ReadToEndAsync();

                log.LogInformation(requestBody);

                UserWithSalons userWithSalons = JsonConvert.DeserializeObject<UserWithSalons>(requestBody);

                _usersRepo.UpdateUser(userWithSalons.user);
                var userSalons = userWithSalons.selectedSalons.Select(s => new UserXSalon() { UserId = userWithSalons.user.Id, SalonId = int.Parse(s.Id) }).ToList();
                _usersRepo.UpdateUserSalons(userSalons);

                return new JsonResult(new { status = "success" });
            }

            return new BadRequestResult();
        }
    }
}
