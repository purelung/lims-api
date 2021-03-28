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
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace ZeeReportingApi.Data
{


    public class Salon
    {
        public string Id { get; set; }
        public string Name
        {
            get; set;
        }
    }

    public class SalonsRepo
    {
        public static List<Salon> GetSalons(int franchiseId, ILogger log = null)
        {
            var salons = new List<Salon> { };

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                using (SqlConnection connection = new SqlConnection(Constants.DB_CONN_STRING))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("app.sel_Salons", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@franchiseID", SqlDbType.Int).Value = franchiseId;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                salons.Add(new Salon()
                                {
                                    Id = reader.GetString(0),
                                    Name = reader.GetString(1),
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return salons;
        }
    }

}