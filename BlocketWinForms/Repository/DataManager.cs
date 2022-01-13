using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BlocketWinForms.Repository
{
    public class DataManager
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["BlocketDBconn"].ConnectionString;

        public static DataTable ExecuteReturnTable(string sqlQuery, List<SqlParameter> parameters)
        {
            // Step 1. Create the connection 
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Step 2. Create the command (anrop)
                SqlCommand command = new SqlCommand(sqlQuery, connection);

                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }               

                // Step 3. Run the command and recieve the returned value/s
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable returnValues = new DataTable();                          

                adapter.Fill(returnValues);

                return returnValues;
            }
        }
    }
}
