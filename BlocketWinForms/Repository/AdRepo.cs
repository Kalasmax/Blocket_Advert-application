using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BlocketWinForms.Repository
{
    public class AdRepo
    {
        string sqlQuery;

        public DataTable SearchAd(string condition, string category)
        {   // Search is only based on the condition (ALL CATEGORIES) if "Kategorier" is selected as category
            if (category == "Kategorier")
            {              
                sqlQuery = "EXEC SearchAllCategories @condition";                                                                                                     
            }
            // Search is only based on the category (ALL ITEMS WITHIN SELECTED CATEGORY) - combobox on top page for example
            else if (string.IsNullOrEmpty(condition))
            {         
                sqlQuery = "EXEC SearchNoCondition @category";                    
            }
            // Search based on both condition and category 
            else
            {               
                sqlQuery = "EXEC SearchConditionAndCategory @condition, @category";                          
            }

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@condition", "%" + condition + "%"),
                new SqlParameter("@category", "%" + category + "%")
            };

            return DataManager.ExecuteReturnTable(sqlQuery, parameters);     
        }
        public void UploadAd(List<string> posting)
        {                   
            sqlQuery = "EXEC UploadAd @username, @categoryname, @title, @details, @price";

            List<SqlParameter> parameters = new List<SqlParameter>
            {              
                new SqlParameter("@username", posting[0]),
                new SqlParameter("@categoryname", posting[2]),
                new SqlParameter("@title", posting[3]),
                new SqlParameter("@details", posting[4] + " Telefon: " + posting[1]),
                new SqlParameter("@price", Convert.ToInt32(posting[5]))
            };

            DataManager.ExecuteReturnTable(sqlQuery, parameters);
        }
        public DataTable DisplayMyAds(string username)
        {
            sqlQuery = "EXEC DisplayMyAds @username";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@username", username)
            };

            return DataManager.ExecuteReturnTable(sqlQuery, parameters);
        }
        public void DeleteAd(int adID)
        {
            sqlQuery = "DELETE FROM Ads WHERE AdID = @adid";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@adid", adID)
            };

            DataManager.ExecuteReturnTable(sqlQuery, parameters);
        }
        public DataTable RetrieveMyAd(int adID, string username)
        {
            sqlQuery = "EXEC RetrieveAd @adid, @username";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@adid", adID),
                new SqlParameter("@username", username)
            };

            return DataManager.ExecuteReturnTable(sqlQuery, parameters);
        }
        public DataTable RetrieveAd(string category, string title, string price)
        {
            sqlQuery = "EXEC ViewAd @category, @title, @price";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
               new SqlParameter("@category", category),
               new SqlParameter("@title", title),
               new SqlParameter("@price", price)
            };

            return DataManager.ExecuteReturnTable(sqlQuery, parameters);
        }
    }
}
