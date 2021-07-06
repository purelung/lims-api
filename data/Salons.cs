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
    public class Salon
    {
        public string Id { get; set; }
        public string Name
        {
            get; set;
        }
    }

    public class SalonWithGroup
    {

        public int Id { get; set; }
        public string Name
        {
            get; set;
        }
        public string Group
        {
            get; set;
        }
    }

    public class UserWithSalons
    {
        public User user { get; set; }
        public List<Salon> availableSalons { get; set; }
        public List<Salon> selectedSalons { get; set; }
    }

    public class SalonsRepo
    {

        private readonly ODSContext _context;
        public SalonsRepo(ODSContext context)
        {
            _context = context;
        }
        public void SaveUserSalons(List<UserXSalon> userSalons)
        {
            _context.UserXSalon.AddRange(userSalons);
            _context.SaveChanges();
        }

        public UserWithSalons GetUserWithSalons(User user)
        {
            var salonIds = _context.UserXSalon.Where(us => us.UserId == user.Id).Select(us => us.SalonId.ToString()).ToList();
            var allSalons = GetSalons(user.FranchiseId);
            var selectedSalons = allSalons.Where(s => salonIds.Contains(s.Id)).ToList();

            return new UserWithSalons() { user = user, selectedSalons = selectedSalons, availableSalons = allSalons.Except(selectedSalons).ToList() };
        }

        public List<Salon> GetSalons(int franchiseId)
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

        public List<SalonWithGroup> GetSalonsWithGroups(string loggedInUserEmail)
        {
            var salonGroups = new List<SalonWithGroup> { };

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                using (SqlConnection connection = new SqlConnection(Constants.DB_CONN_STRING))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("[reports].[SalonGroups]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@LoggedInUserEmail", SqlDbType.NVarChar).Value = loggedInUserEmail;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                salonGroups.Add(new SalonWithGroup()
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Group = reader.GetString(2),
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

            return salonGroups;
        }
    }
}