using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using AzFunctionsJwtAuth;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;

namespace ZeeReportingApi
{
    public struct Customer
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string OwnerName { get; set; }
    }

    public class getCustomers : AuthorizedServiceBase
    {
        [FunctionName("getCustomers")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            log.LogInformation(requestBody);

            var customers = getCustomersDb();

            return new JsonResult(customers);
        }

        private List<Customer> getCustomersDb()
        {
            var customers = new List<Customer> { };

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            // builder.DataSource = "tcp:midnight-data.database.windows.net,1433;Initial Catalog=ODS;Persist Security Info=False;User ID=midnightData;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            // builder.UserID = "<your_username>";
            // builder.Password = "<your_password>";
            // builder.InitialCatalog = "<your_database>";
            try
            {
                using (SqlConnection connection = new SqlConnection("Server=tcp:midnight-data.database.windows.net,1433;Initial Catalog=ODS;Persist Security Info=False;User ID=midnightData;Password=KRCxb24jLgzat279TgtYKPf4q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();

                    String sql = "SELECT id, customerName, ownerName FROM [app].[Customer]";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1} {2}", reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                                customers.Add(new Customer() { Id = reader.GetInt32(0), CustomerName = reader.GetString(1), OwnerName = reader.GetString(2) });
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            // Console.WriteLine("\nDone. Press enter.");
            // Console.ReadLine();

            return customers;
        }
    }
}
