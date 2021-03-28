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

namespace ZeeReportingApi.Data
{
    public enum Role
    {
        SuperUser = 1,
        Owner = 2,
        SalonOwner = 3,
        User = 4
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int FranchiseId { get; set; }
        public Role Role { get; set; }
    }

    public class UsersRepo
    {
        private static readonly string BASE_QUERY = "select id, Name, Email, Mobile, FranchiseID, UserRoleID as Role from [app].[User]";

        public static bool AddUser(User user)
        {
            bool succuess = false;

            string query = String.Format(@"INSERT INTO app.[User] (Name, Email, Mobile, Password, FranchiseID, UserRoleID)
                VALUES ('{0}', '{1}', '{2}', '', {3}, {4});", user.Name, user.Email, user.Mobile, user.FranchiseId, (int)user.Role);

            try
            {
                using (SqlConnection connection = new SqlConnection(Constants.DB_CONN_STRING))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected != 1)
                        {
                            return false;
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        private static List<User> getUsersBase(string query)
        {
            var users = new List<User> { };

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                using (SqlConnection connection = new SqlConnection(Constants.DB_CONN_STRING))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new User()
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Mobile = reader.GetString(3),
                                    FranchiseId = reader.GetInt32(4),
                                    Role = (Role)reader.GetInt32(5)
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

            return users;
        }


        public static List<User> GetUser(string email)
        {
            return getUsersBase(BASE_QUERY + string.Format(" where Email = '{0}'", email));
        }

        public static List<User> GetUsers()
        {
            return getUsersBase(BASE_QUERY);
        }
    }
}
