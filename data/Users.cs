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
        SuperUser,
        Owner,
        SalonOwner,
        User
    }

    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public bool AcceptedInvite { get; set; }
        public DateTime InviteSent { get; set; }
        public Role Role { get; set; }
    }

    public class UsersRepo
    {
        public static List<User> get(string email)
        {
            var users = new List<User> { };

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                using (SqlConnection connection = new SqlConnection("Server=tcp:midnight-data.database.windows.net,1433;Initial Catalog=ODS;Persist Security Info=False;User ID=midnightData;Password=KRCxb24jLgzat279TgtYKPf4q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
                {
                    connection.Open();

                    String sql = string.Format(@"select u.Name, u.Email, u.Mobile, u.AcceptedInvite, u.InviteSent, r.RoleDescription as Role from [app].[User] as u join app.UserRoles as r on u.UserRoleID = r.id
                                where u.Email = '{0}'", email);

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new User()
                                {
                                    Name = reader.GetString(0),
                                    Email = reader.GetString(1),
                                    Mobile = reader.GetString(2),
                                    AcceptedInvite = reader.GetBoolean(3),
                                    InviteSent = reader.GetDateTime(4),
                                    Role = Enum.Parse<Role>(reader.GetString(5))
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
    }
}
