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


    public struct SprocParam
    {
        public string Name { get; set; }
        public Object Value { get; set; }
        public SqlDbType DbType { get; set; }
    }

    public class DataUtility
    {
        public static readonly string[] DefaultParams = { "@LoggedInUserEmail", "@StartDate", "@EndDate" };

        public static List<SprocParam> GetSprocParams(string[] paramNames, string userName)
        {
            var sprocParams = new List<SprocParam>();

            var now = DateTime.Now;
            var oneWeekAgo = now.AddDays(-7);

            paramNames.ToList().ForEach(paramName =>
            {
                switch (paramName)
                {
                    case "@LoggedInUserEmail":
                        sprocParams.Add(new SprocParam()
                        {
                            Name = paramName,
                            Value = userName,
                            DbType = SqlDbType.NVarChar
                        });
                        break;
                    case "@StartDate":

                        sprocParams.Add(new SprocParam()
                        {
                            Name = paramName,
                            Value = now,
                            DbType = SqlDbType.DateTime
                        });
                        break;
                    case "@EndDate":

                        sprocParams.Add(new SprocParam()
                        {
                            Name = paramName,
                            Value = oneWeekAgo,
                            DbType = SqlDbType.DateTime
                        });
                        break;
                    default:
                        break;
                }
            });

            return sprocParams;
        }

        public static List<Dictionary<string, object>> CallSproc(string sprocName, string userName)
        {
            return CallSproc(sprocName, userName, DefaultParams);
        }

        public static List<Dictionary<string, object>> CallSproc(string sprocName, string userName,
        string[] sprocParamNames)
        {
            var items = new List<Dictionary<string, object>>();
            var sprocParams = GetSprocParams(sprocParamNames, userName);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                using (SqlConnection connection = new SqlConnection(Constants.DB_CONN_STRING))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sprocName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        sprocParams.ForEach(p =>
                        {
                            command.Parameters.Add(p.Name, p.DbType).Value = p.Value;
                        });

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

                            while (reader.Read())
                            {
                                var item = new Dictionary<string, Object>();
                                items.Add(item);

                                for (int i = 0; i < reader.FieldCount; i++)
                                    item.Add(columns[i], reader[i]);
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return items;
        }


    }
}