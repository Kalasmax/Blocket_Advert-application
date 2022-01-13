using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BlocketWinForms.Repository
{
    public class UserRepo
    {
        public DataTable Search(string username, string password)
        {           
            string sqlQuery;          
            // Checks if there is a user with entered username
            if (string.IsNullOrEmpty(password))
            {              
                sqlQuery = "EXEC SearchUserExists @username";
            }           
            // Checks if there is a user with entered username + password
            else
            {
                sqlQuery = "EXEC CheckCredentials @username, @password";
            }

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@username", username),
                new SqlParameter("@password", password)
            };

            return DataManager.ExecuteReturnTable(sqlQuery, parameters);
        }

        public void Register(string username, string password)
        {
            string sqlQuery = "INSERT INTO Users (Username, Countersign) " +
                              "VALUES(@username, @password)";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@username", username),
                new SqlParameter("@password", password)
            };

            DataManager.ExecuteReturnTable(sqlQuery, parameters);
        }
    }
}
