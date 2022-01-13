using BlocketWinForms.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BlocketWinForms.Classes
{
    public class Ad
    {
        AdRepo repo = new AdRepo();

        public string PageTitle { get; set; }

        public DataTable SearchAd(string condition, string category)
        {   // Search is only based on the condition (ALL CATEGORIES) if "Kategorier" is selected as category
            if (category == "Kategorier")
            {
                if (condition.StartsWith("🔎"))
                {
                    condition = "";
                }             
                PageTitle = "Säljes";
            }
            // Search is only based on the category (ALL ITEMS WITHIN SELECTED CATEGORY)
            else if (string.IsNullOrWhiteSpace(condition) || condition.StartsWith("🔎"))
            {           
                condition = "";
                PageTitle = $"{category} säljes";           
            }           
            // Search based on both condition and category  
            else
            {
                PageTitle = $"{category} säljes";
            }                  

            return repo.SearchAd(condition, category);
        }

        public void UploadAd(List<string> posting)
        {
            repo.UploadAd(posting);
        }

        public DataTable DisplayMyAds(string username)
        {
            return repo.DisplayMyAds(username);
        }

        public void DeleteAd(object adID)
        {
            repo.DeleteAd(Convert.ToInt32(adID));
        }

        public List<DataRow> RetrieveMyAd(object adID, string username)
        {                    
            return repo.RetrieveMyAd(Convert.ToInt32(adID), username).AsEnumerable().ToList();
        }

        public List<DataRow> RetrieveAd(string category, string title, string price)
        {
            return repo.RetrieveAd(category, title, price).AsEnumerable().ToList();
        }
    }
}
