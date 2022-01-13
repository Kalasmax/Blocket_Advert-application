using BlocketWinForms.Repository;
using System.Data;

namespace BlocketWinForms.Classes
{
    public class User
    {
        public bool SignedIn { get; set; }       

        UserRepo repo = new UserRepo();
        DataTable returnValue;

        public int Search(string username, string password)
        {       
            returnValue = repo.Search(username, password);
            int match = 0;
            
            if (string.IsNullOrEmpty(password))
            {              
                foreach (var row in returnValue.AsEnumerable())
                {
                    match = row.Field<int>("Match");
                }
            }          
            else
            {             
                foreach (var row in returnValue.AsEnumerable())
                {                  
                    match = row.Field<int>("Match2");                  
                }          
            }
              
            return match;
        }

        public void Register(string username, string password)
        {
            repo.Register(username, password);
        }
    }
}
