using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlocketWinForms.Classes
{
    public class InputOutputManager
    {
        //NOT the smartest nor prettiest solution to even allow users to even enter invalid characters
        //to the phone number string for example. but im doing a replica of blockets webpage.
        public (List<string>, string[], int) InputChecker(List<string> detailsToCheck) // [0] Username, [1] Phone, [2] Category, [3] Title, [4] Description(text), [5] Price
        {
            string[] errorMsg = new string[6];  // 0 = invalid | 1 = valid | 2 = adjust #1 | 3 = adjust #2 | 4 = adjust #3 | 5 = adjust #4 | 6 = adjust #5    
            Regex invalidChars = new Regex("[A-Öa-ö¤%&/()=?´`^_]");               
            int errorCount = 0;
            int counter = 0;

            foreach (string detail in detailsToCheck.ToList())
            {
                #region Empty fields 
                // OM STRÄNGEN ÄR TOM ELLER BARA MELLANLSAG
                if (string.IsNullOrEmpty(detail) || string.IsNullOrWhiteSpace(detail))
                {                     
                    if (detail != detailsToCheck[5]) // OM STRÄNGEN INTE ÄR "Price" = ICKE GODKÄNT (TELEFON, TITEL, BESKRIVNING FÅR INTE VARA TOM)
                    {                       
                        errorCount ++;
                        switch (counter)
                        {
                            case 1:
                                errorMsg[counter] = "Skriv ett telefonnummer";
                                break;
                            case 3:
                                errorMsg[counter] = "Skriv en rubrik";
                                break;
                            case 4: 
                                errorMsg[counter] = "Skriv en annonstext";
                                break;
                        }
                    }
                    #region Price 1
                    else if (detail == detailsToCheck[5]) // ANNARS OM STRÄNGEN ÄR "Price" = GODKÄNT (PRISET FÅR VARA 0)
                    {
                        detailsToCheck[5] = detailsToCheck[5].Trim();
                        errorMsg[5] = "Godkänt";
                    }
                    #endregion
                }
                else
                {
                    errorMsg[counter] = "Godkänt";
                }
                #endregion
                #region Category               
                if (detail == "Kategorier") // OM KATEGORI ÄR "Kategori"
                {
                    if (detailsToCheck[2] == "Kategorier")
                    {
                        errorMsg[2] = "Välj en kategori";  
                        errorCount++;
                    }
                    else
                    {
                        errorMsg[2] = "Godkänt"; 
                    }
                }
                #endregion
                #region Phone 
                // HANTERAR OGILTIGA TECKEN OCH FÖR LÅNG/KORT INMATNING I TELEFON 
                if (counter == 1 && errorMsg[1] != "Skriv ett telefonnummer")
                {
                                                                         
                    if (detailsToCheck[1].StartsWith("0") || detailsToCheck[1].StartsWith("+")) // OM TELEFONNUMRET BÖRJAR MED 0 ELLER +
                    {
                        if (detailsToCheck[1].StartsWith("0")) // OM TELEFONNUMRET BÖRJAR MED 0
                        {
                            if (detail.Substring(0, 2) == "00") // OM DE 2 FÖRSTA SIFFRORNA ÄR 00 
                            {
                                detailsToCheck[1] = detailsToCheck[1].Replace("00", "+"); // BYTS DE UT MOT +
                            }
                        }
                        if (detailsToCheck[1].StartsWith("+")) // OM TELEFONNUMRET BÖRJAR MED +
                        {
                            if (detailsToCheck[1].Substring(1, 2) == "46") // OM DE 2 FÖLJANDE SIFFRORNA EFTER +TECKNET ÄR 46 (SVERIGES RIKTNUMMER)
                            {
                                detailsToCheck[1] = detailsToCheck[1].Replace("+46", "0"); // BYTS DE UT MOT SIFFRAN 0
                            }
                        }
                    }

                    detailsToCheck[1] = Regex.Replace(detailsToCheck[1], "[^0-9!#%.'-*]", ""); // TRIMMAR TELEFONNUMRET FRÅN ("mellanslag" ! # % . - ' * , " )

                    while (true)
                    {
                        if (invalidChars.IsMatch(detailsToCheck[1])) // OM TELEFONNUMRET INNEHÅLLER OGILTIGA TECKEN ( A-Ö  ¤ % & / ( ) = ? ´ ` ^ _  )
                        {
                            errorMsg[1] = "Ta bort ogiltiga tecken";
                            errorCount++;
                            break;
                        }                       
                        else if (detailsToCheck[1].Substring(0,1) != "0" && detailsToCheck[1].Substring(0, 1) != "+") //OM TELEFONNUMRET INTE BÖRJAR MED 0 eller +
                        {
                            errorMsg[1] = "Ange ett korrekt riktnummer";
                            errorCount++;
                            break;
                        }                     
                        else if (detail.Count(a => a == '+') > 1) // OM TELEFONNUMRET INNEHÅLLER MER ÄN 1 +
                        {
                            errorMsg[1] = "Kontrollera telefonnumret - det innehåller för många plustecken.";
                            errorCount++;
                            break;
                        }
                        else if (detail.Length > 12) // OM TELEFONNUMRET ÄR LÄNGRE ÄN 12 SIFFROR
                        {
                            errorMsg[1] = "Kontrollera telefonnumret - det är för långt";
                            errorCount++;
                            break;
                        }
                        else if (detail.Length < 8) // OM TELEFONNUMRET ÄR KORTARE ÄN 8 SIFFROR
                        {
                            errorMsg[1] = "Kontrollera telefonnumret - det innehåller för få siffror";
                            errorCount++;
                            break;
                        }
                        else
                        {
                            errorMsg[1] = "Godkänt";
                            break;
                        }
                        
                    }
                }
                #endregion
                #region Title and Text  
                if (counter == 3 && errorMsg[3] != "Skriv en rubrik" || counter == 4 && errorMsg[4] != "Skriv en annonstext") // OM TITEL / BESKRIVNING 
                {                                                             
                    if (detail.Length < 2) // TITEL OCH BESKRIVNING
                    {                         
                        if (detail == detailsToCheck[3] && detail.Length < 2) // OM TITEL ÄR KORTARE ÄN 2 TECKEN
                        {
                            errorMsg[3] = "Skriv minst två bokstäver";
                            errorCount++;
                        }
                        else if (detail == detailsToCheck[4] && detail.Length < 2) // OM BESKRIVNING ÄR KORTARE ÄN 2 TECKEN
                        {
                            errorMsg[4] = "Skriv minst två bokstäver";
                            errorCount++;
                        }
                    }                       
                }
                #endregion
                #region Price 2
                if (counter == 5)
                {
                    detailsToCheck[5] = detailsToCheck[5].Trim();
                }
                #endregion 
                counter++;
            }
            return (detailsToCheck, errorMsg, errorCount);                   
        }

        public List<string> OutputChecker(List<DataRow> retrievedAd, string key)
        {
            DataRow itemArray = retrievedAd[0];
            List<string> addDetail = new List<string>();

            if (key == "full")
            {            
                //Phone
                string descPhone = itemArray.ItemArray[2].ToString();
                int length = descPhone.ToString().Length;
                int pos = descPhone.LastIndexOf(' ');
                addDetail.Add(descPhone.Substring(pos, length - pos).TrimStart());
                //Category
                addDetail.Add(itemArray[0].ToString());
                //Title
                addDetail.Add(itemArray[1].ToString());
                //Details
                addDetail.Add(descPhone.Substring(0, pos - 9));
                //Price
                addDetail.Add(" " + itemArray[3].ToString());
                //Username
                addDetail.Add(itemArray[4].ToString());
            }
            else if (key == "half")
            {            
                //Phone
                string descPhone = itemArray.ItemArray[0].ToString();
                int length = descPhone.ToString().Length;
                int pos = descPhone.LastIndexOf(' ');
                addDetail.Add(descPhone.Substring(pos, length - pos).TrimStart());
                //Details
                addDetail.Add(descPhone.Substring(0, pos - 9));
                //Username
                addDetail.Add(itemArray[1].ToString());
            }
            
            return addDetail;
        }
    }
}
