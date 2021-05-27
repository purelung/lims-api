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


    public class SprocParam
    {
        public string Name { get; set; }
        public Object Value { get; set; }
        public SqlDbType DbType { get; set; }

        public override string ToString()
        {
            var value = "";
            if (Value.GetType() == typeof(DateTime))
            {
                value = ((DateTime)Value).ToShortDateString();
            }
            else
            {
                value = Value.ToString();
            }
            return Name + ":" + value;
        }
    }



    public class DataUtility
    {
        public static readonly string[] CachedSprocs = { "reports.dashboardMetrics" };

        public static Dictionary<string, List<Dictionary<string, object>>> SprocResultsCache = new Dictionary<string, List<Dictionary<string, object>>>();

        public static SprocParam GetStartDate(DateTime value)
        {
            return new SprocParam()
            {
                Name = "@StartDate",
                Value = value,
                DbType = SqlDbType.DateTime
            };
        }

        public static SprocParam GetEndDate(DateTime value)
        {
            return new SprocParam()
            {
                Name = "@EndDate",
                Value = value,
                DbType = SqlDbType.DateTime
            };
        }

        public static SprocParam GetUser(string value)
        {
            return new SprocParam()
            {
                Name = "@LoggedInUserEmail",
                Value = value,
                DbType = SqlDbType.NVarChar
            };
        }

        public static List<SprocParam> GetDefaultSprocParams(string userName, bool useDate)
        {
            var sprocParams = new List<SprocParam>();

            var now = DateTime.Now;
            var oneWeekAgo = now.AddDays(-7);

            var defaultParams = useDate ? new[] { "@LoggedInUserEmail", "@StartDate", "@EndDate" } : new[] { "@LoggedInUserEmail" };

            defaultParams.ToList().ForEach(paramName =>
            {
                switch (paramName)
                {
                    case "@LoggedInUserEmail":
                        sprocParams.Add(GetUser(userName));
                        break;
                    case "@StartDate":
                        sprocParams.Add(GetStartDate(oneWeekAgo));
                        break;
                    case "@EndDate":
                        sprocParams.Add(GetEndDate(now));
                        break;
                    default:
                        break;
                }
            });

            return sprocParams;
        }

        public static string GetSprocId(string sprocName, List<SprocParam> sprocParams)
        {
            return sprocName + sprocParams.Aggregate("",
                (current, next) => current.ToString() + "-" + next.ToString());
        }

        public static List<Dictionary<string, object>> CallSproc(string sprocName, string userName, bool useDate = true)
        {
            return CallSproc(sprocName, GetDefaultSprocParams(userName, useDate));
        }

        public static List<Dictionary<string, object>> CallSproc(string sprocName,
        List<SprocParam> sprocParams)
        {
            var items = new List<Dictionary<string, object>>();

            var sprocId = GetSprocId(sprocName, sprocParams);

            if (CachedSprocs.Contains(sprocName) && SprocResultsCache.ContainsKey(sprocId))
            {
                Console.WriteLine("Retrieving sproc results from cache: " + sprocId);
                return SprocResultsCache[sprocId];
            }

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

            if (CachedSprocs.Contains(sprocName))
            {
                Console.WriteLine("Adding sproc results to cache: " + sprocId);
                SprocResultsCache.Add(sprocId, items);
            }

            return items;
        }


    }
}