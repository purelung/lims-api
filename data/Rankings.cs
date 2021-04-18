using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ZeeReportingApi.Model;

namespace ZeeReportingApi.Data
{
    public class Ranking
    {
        public int SalonId { get; set; }
        public int CustomerCount { get; set; }
        public decimal CPH { get; set; }
        public decimal SPH { get; set; }
        public decimal RPC { get; set; }
        public decimal ProductNet { get; set; }
        public decimal ColorAmount { get; set; }
        public int ColorCount { get; set; }
        public decimal AvgTicket { get; set; }
    }

    public class RankingsRepo
    {

        private readonly ODSContext _context;
        public RankingsRepo(ODSContext context)
        {
            _context = context;
        }

        public List<Ranking> GetRankings(string userEmail)
        {
            var rankings = new List<Ranking> { };

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                using (SqlConnection connection = new SqlConnection(Constants.DB_CONN_STRING))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("[reports].[ranking]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@LoggedInUserEmail", SqlDbType.NVarChar).Value = userEmail;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rankings.Add(new Ranking()
                                {
                                    SalonId = Int32.Parse(reader.GetString(0)),
                                    CustomerCount = reader.GetInt32(1),
                                    CPH = reader.GetDecimal(2),
                                    SPH = reader.GetDecimal(3),
                                    RPC = reader.GetDecimal(4),
                                    ProductNet = reader.GetDecimal(5),
                                    ColorAmount = reader.GetDecimal(6),
                                    ColorCount = reader.GetInt32(7),
                                    AvgTicket = reader.GetDecimal(8),
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

            return rankings;
        }
    }
}