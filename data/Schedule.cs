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
    public class Schedule
    {
        public int SalonId { get; set; }
        public string Description { get; set; }
        public int Sunday { get; set; }

        public int Monday { get; set; }
        public int Tuesday { get; set; }
        public int Wednesday { get; set; }
        public int Thursday { get; set; }
        public int Friday { get; set; }
        public int Saturday { get; set; }
        public int WeekTotal { get; set; }
    }

    public class SchedulesRepo
    {

        private readonly ODSContext _context;
        public SchedulesRepo(ODSContext context)
        {
            _context = context;
        }

        public List<Schedule> GetSchedules(string userEmail)
        {
            var schedules = new List<Schedule> { };

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                using (SqlConnection connection = new SqlConnection(Constants.DB_CONN_STRING))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("[reports].[schedule]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@LoggedInUserEmail", SqlDbType.NVarChar).Value = userEmail;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                schedules.Add(new Schedule()
                                {
                                    SalonId = Int32.Parse(reader.GetString(0)),
                                    Description = reader.GetString(1),
                                    Sunday = reader.GetInt32(2),
                                    Monday = reader.GetInt32(3),
                                    Tuesday = reader.GetInt32(4),
                                    Wednesday = reader.GetInt32(5),
                                    Thursday = reader.GetInt32(6),
                                    Friday = reader.GetInt32(7),
                                    Saturday = reader.GetInt32(8),
                                    WeekTotal = reader.GetInt32(9),
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

            return schedules;
        }
    }
}