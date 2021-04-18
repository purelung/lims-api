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

    public class DataUtility
    {


        public static List<Dictionary<string, object>> CallSproc<T>(string sprocName, string paramName, T param, SqlDbType paramType)
        {
            var items = new List<Dictionary<string, object>>();

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            try
            {
                using (SqlConnection connection = new SqlConnection(Constants.DB_CONN_STRING))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sprocName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(paramName, paramType).Value = param;
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