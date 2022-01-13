using BlocketWinForms.Classes;
using BlocketWinForms.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace BlocketWinForms.Views
{
    public partial class MainForm : Form
    {
        User user = new User();
        Ad ad = new Ad();
        InputOutputManager manager = new InputOutputManager();

        public MainForm()
        {
            InitializeComponent();           
        }

        #region Search methods

        //Top panel - Searches for all items within selected category, without condition
        private void CategoryTopSelected(object sender, EventArgs e)
        {          
            SearchMethod("", cmbCategoryTopPagePnl.Text);          
        }
        //Search panel - Searches for items within selected category and condition      
        private void SearchClick(object sender, EventArgs e)
        {           
            SearchMethod(txtConditionSearchPnl.Text, cmbCategorySearchPnl.Text);          
        }
        //Results panel - Searches for items within selected category and condition when item is selected in the combobox
        private void CategoryResultSelected(object sender, EventArgs e)
        {
            SearchMethod(txtConditionResultsPnl.Text, cmbCategoryResultsPnl.Text);
        }
        //Method taking condition and category as parameters and searches the database
        public void SearchMethod(string condition, string category)
        {          
            DataTable returnValues = ad.SearchAd(condition, category);
            dgvResults.DataSource = returnValues;
            if (returnValues.Rows.Count < 1)
            {
                EnterResultPage("noMatch");
                lblNoResultMsg.Text = "Inga annonser för " + condition + " hittades";
            }
            else
            {
                EnterResultPage("match");
                //Sorts the results by latest added ad first and descending
                dgvResults.Sort(dgvResults.Columns["DateAdded"], ListSortDirection.Descending);
            }         
            //Changes the Text of the Form depending on what is searched after
            Text = $"Blocket - {ad.PageTitle} i hela Sverige";
        }      

        #endregion

        #region Sorting methods

        //Combobox that changes the sorting of the results
        private void ChangeSorting(object sender, EventArgs e)
        {
            if (cmbSortResults.Text == "Senaste")
            {
                dgvResults.Sort(dgvResults.Columns["DateAdded"], ListSortDirection.Descending);
            }
            if (cmbSortResults.Text == "Äldst")
            {
                dgvResults.Sort(dgvResults.Columns["DateAdded"], ListSortDirection.Ascending);
            }
            if (cmbSortResults.Text == "Billigast")
            {
                dgvResults.Sort(dgvResults.Columns["Price"], ListSortDirection.Ascending);
            }
            if (cmbSortResults.Text == "Dyrast")
            {
                dgvResults.Sort(dgvResults.Columns["Price"], ListSortDirection.Descending);
            }
        }

        #endregion

        #region Select ad methods

        //Selecting ad from the results grid view
        private void ViewAd(object sender, EventArgs e)
        {
            if (dgvResults.SelectedCells.Count > 0)
            {
                try                           
                {                 
                    int index = dgvResults.SelectedCells[0].RowIndex;
                    string category = dgvResults.Rows[index].Cells[0].Value.ToString();
                    string dateAdded = dgvResults.Rows[index].Cells[3].Value.ToString();
                    string title = dgvResults.Rows[index].Cells[1].Value.ToString();
                    string price = dgvResults.Rows[index].Cells[2].Value.ToString();
                    EnterViewPanel();
                    FillAdInfo(ad.RetrieveAd(category, title, price), "");                  
                    lblAdCategory.Text = category;
                    lblAdDateAdded.Text = "🕘 Inlagd: " + dateAdded.Substring(0, 16);
                    lblAdTitle.Text = title;
                    lblAdPrice.Text = price + " kr";
                }
                catch (Exception) //Bug fix
                {
                }
            }  
        }

        #endregion

        #region Log in methods           

        private void ClickContinue(object sender, EventArgs e)
        {
            if (txtUsername.Text == string.Empty || txtUsername.Text.StartsWith("👤"))
            {
                UsernameInputError();
            }
            else if (txtPassword.Text == string.Empty || txtPassword.Text == "Ange ditt lösenord *" && txtPassword.Visible == true && txtPassword.Enabled == true)
            {
                PasswordInputError();
            }
            else if (btnContinue.Text == "Logga in")
            {
                InitiateLogIn(txtUsername.Text);
            }
            else if (btnContinue.Text == "Skapa konto")
            {
                InitiateRegistration(txtUsername.Text);
            }
            else
            {
                int match = user.Search(txtUsername.Text, string.Empty);
                // When entered username is NOT matched with a registered username from db
                if (match == 0)
                {
                    InitiateRegistration(string.Empty);
                }
                // When entered username is matched with a registered username from db
                else
                {
                    InitiateLogIn(string.Empty);
                }
            }
        }
        public void InitiateLogIn(string username)
        {
            if (username == string.Empty)
            {
                SetLogInPanel();
            }
            else
            {
                int match = user.Search(username, txtPassword.Text);
                if (match == 0)
                {
                    this.Opacity = .80;
                    WrongPasswordForm wrongPasswordForm = new WrongPasswordForm();
                    wrongPasswordForm.Show();
                }
                else
                {
                    EnterUserPage(username);                   
                    pnlTopPageButtons.Visible = true;
                }
            }
        }

        #endregion

        #region User page/My ads methods

        private void ViewMyAd(object sender, EventArgs e)
        {
            if (dgvMyAds.SelectedCells.Count > 0)
            {
                try
                {                  
                    int index = dgvMyAds.SelectedCells[0].RowIndex;
                    EnterViewPanel();
                    lblAdDateAdded.Text = "🕘 Inlagd: " + dgvMyAds.Rows[index].Cells[3].Value.ToString().Substring(0, 16);
                }
                catch (Exception) //Bug fix
                {
                }
                DataGridViewRow selectedAd = SelectedRowIndex();              
                FillAdInfo(ad.RetrieveMyAd(selectedAd.Cells["AdId"].Value, lblUserName.Text), "view");            
            }             
        }
        private void UpdateAd(object sender, EventArgs e)
        {
            if (dgvMyAds.SelectedCells.Count > 0)
            {             
                DataGridViewRow selectedAd = SelectedRowIndex();
                lblAdId.Text = selectedAd.Cells["AdId"].Value.ToString();
                EnterAddAdPage("update");
                FillAdInfo(ad.RetrieveMyAd(selectedAd.Cells["AdId"].Value, lblUserName.Text), "update");
            }
        }
        
        private void DeleteAd(object sender, EventArgs e)
        {
            if (dgvMyAds.SelectedCells.Count > 0)
            {
                DataGridViewRow selectedAd = SelectedRowIndex();
                ad.DeleteAd(selectedAd.Cells["AdId"].Value);
                DisplayMyAds();
            }         
        }
        private DataGridViewRow SelectedRowIndex()
        {          
            int selectedRowIndex = dgvMyAds.SelectedCells[0].RowIndex;
            return dgvMyAds.Rows[selectedRowIndex];
        }

        #endregion

        #region Add ad methods

        private void PreviewAd(object sender, EventArgs e)
        {
            EnterViewPanel();
            lblAdCategory.Text = cmbCategoryAddAd.Text;
            lblAdDateAdded.Text = DateTime.Now.ToString().Substring(0, 16);
            lblAdTitle.Text = txtTitleAddAd.Text;
            lblAdPrice.Text = txtPriceAddAd.Text.TrimStart() + " kr";
            lblAdUsername.Text = txtNameAddAd.Text;
            btnShowPhone.Text = "📞 " + txtPhoneAddAd.Text;
            txtAdDescription.Text = txtTextAddAd.Text;
        }

        private void UploadAdClick(object sender, EventArgs e)
        {                     
            //Creates list
            List<string> posting = new List<string>
            {
                txtNameAddAd.Text,
                txtPhoneAddAd.Text,
                cmbCategoryAddAd.Text,
                txtTitleAddAd.Text,
                txtTextAddAd.Text,
                txtPriceAddAd.Text
            };
            (List<string> correctedPosting, string[] errorKey, int errorCount) = manager.InputChecker(posting);    
            
            if (chkTerms.Checked && errorCount == 0)
            {
                if (lblAdId.Visible == true && lblAdId.Text != string.Empty)
                {
                    ad.DeleteAd(lblAdId.Text);
                }
                ad.UploadAd(correctedPosting);
                EnterUserPage("");
            }
            else
            {
                lblTermsAddAdMsg.Visible = true;
                int counter = 0;
                string msg = "";
                foreach (string key in errorKey)
                {
                    msg += "Rad " + counter + ": felmeddelande =" + key + "\n";
                    counter++;
                }
                MessageBox.Show(msg + "\n" + "Antal fel: " + errorCount +", rätta dessa och godkänn villkoren");            
            }       
        }   

        #endregion

        #region Registration methods

        private void InitiateRegistration(string username)
        {
            if (username == string.Empty)
            {
                SetRegistrationPanel();
            }
            else
            {
                user.Register(username, txtPassword.Text);                            
                EnterUserPage(username);
                pnlTopPage.Visible = true;
            }
        }

        #endregion
     
        #region GUI events

        #region View 1 - Main page (pnlSearch)   

        //Removes the text from the search bar when clicked - and returns it when clicking somewhere else
        private void SearchEnter(object sender, EventArgs e)
        {
            txtConditionSearchPnl.Text = "";
        }
        private void SearchLeave(object sender, EventArgs e)
        {
            if (txtConditionSearchPnl.Text == "")
            {
                txtConditionSearchPnl.Text = "🔎 Vad vill du söka efter?";
            }
        }
        //If the ENTER-key is pressed, the linked button is clicked for smoother controls
        private void SearchKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearchPnl.PerformClick();
            }
        }
        private void CategoryKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearchPnl.PerformClick();
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'blocketDBCategories.Categories' table. You can move, or remove it, as needed.
            this.categoriesTableAdapter.Fill(this.blocketDBCategories.Categories);         
        }

        #endregion

        #region View 2 - Search results page (pnlSearchResults)
       
        //Removes or writes the pre-set text in the textbox (in searchresults planel)
        private void SearchInResultsEnter(object sender, EventArgs e)
        {
            if (txtConditionResultsPnl.Text.StartsWith("🔎"))
            {
                txtConditionResultsPnl.Text = "";
            }
        }
        private void SearchInResultsLeave(object sender, EventArgs e)
        {
            if (txtConditionResultsPnl.Text == "")
            {
                txtConditionResultsPnl.Text = "🔎 Vad vill du söka efter?";
            }
        }
        //If enter is pressed, the button is clicked for smoother controls
        private void SearchInResultsKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchMethod(txtConditionResultsPnl.Text, cmbCategoryResultsPnl.Text);
            }
        }
        //Changes the font to Underline when mouse is hovering an ad
        private void CellHoverMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                dgvResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new Font("Calibri", 11, FontStyle.Underline);
            }
        }
        private void CellHoverLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                dgvResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new Font("Calibri", 11, FontStyle.Regular);
            }
        }
        //Switches between pnlSearch to pnlSearchResult
        private void EnterResultPage(string match)
        {
            pnlSearchResults.Visible = true;
            if (match == "match")
            {
                pnlNoResults.Visible = false;
            }
            else if (match == "noMatch")
            {             
                pnlNoResults.Visible = true;
            }                
            //pnlTopPageButtons.Visible = false;
            pnlSearch.Visible = false;
            //pnlSearchResults.Visible = false;           
            pnlAddAd.Visible = false;
            pnlNotSignedIn.Visible = false;
            pnlLogIn.Visible = false;
            pnlUserPage.Visible = false;                              
        }
        //Bug fix
        private void pnlNoResultsClick(object sender, EventArgs e)
        {
            lblTopPagePnlTitle.Focus();
        }

        #endregion

        #region View 2 - Log in/User/Add ad buttons (pnlTopPageButtons)
    
        #region View 2.1 - Log in

        //Changes the bust-icon color to the same as the button when mouse is hovering - and back when the cursor is leaving
        private void LogInHoverEnter(object sender, EventArgs e)
        {
            LogInIconFlick("enter");
        }
        private void LogInHoverLeave(object sender, EventArgs e)
        {
            LogInIconFlick("leave");
        }      
        private void LogInUserIconHoverEnter(object sender, EventArgs e)
        {
            LogInIconFlick("enter");
        }
        private void LogInUserIconHoverLeave(object sender, EventArgs e)
        {
            LogInIconFlick("leave");
        }
        private void LogInIconFlick(string command)
        {
            if (command == "enter")
            {
                btnLogIn.FlatAppearance.MouseOverBackColor = Color.Transparent; btnLogIn.Font = new Font(btnLogIn.Font.Name, 8, FontStyle.Underline);
                btnLogIn.ForeColor = SystemColors.ControlText; lblLogInUserIcon.ForeColor = Color.Black;
            }
            else if (command == "leave")
            {
                btnLogIn.BackColor = SystemColors.Window; btnLogIn.Font = new Font(btnLogIn.Font.Name, 8);
                btnLogIn.ForeColor = SystemColors.GrayText; lblLogInUserIcon.ForeColor = SystemColors.GrayText;
            }
        }
        //Enters the log in page by either clicking on the button or the icon
        private void LogInPageClick(object sender, EventArgs e)
        {
            EnterLogInPage();
        }
        private void LogInIconClick(object sender, EventArgs e)
        {
            EnterLogInPage();
        }
        private void EnterLogInPage()
        {
            Text = "Logga in med ditt Blocket-konto";
            pnlLogIn.Visible = true;

            pnlTopPageButtons.Visible = false; pnlSearch.Visible = false;          
            pnlSearchResults.Visible = false; pnlNoResults.Visible = false;
            pnlAddAd.Visible = false; pnlNotSignedIn.Visible = false;
            pnlUserPage.Visible = false; pnlNoAds.Visible = false;
        }

        #endregion

        #region View 2.2 - User

        //Changes the bust-icon color to the same as the button when mouse is hovering - and back when the cursor is leaving
        private void UserHoverEnter(object sender, EventArgs e)
        {
            UserIconFlick("enter");
        }
        private void UserHoverLeave(object sender, EventArgs e)
        {
            UserIconFlick("leave");
        }
        private void UserIconHoverEnter(object sender, EventArgs e)
        {
            UserIconFlick("enter");
        }
        private void UserIconHoverLeave(object sender, EventArgs e)
        {
            UserIconFlick("leave");
        }
        private void UserIconFlick(string command)
        {
            if (command == "enter")
            {
                btnUser.FlatAppearance.MouseOverBackColor = Color.Transparent; btnUser.Font = new Font(btnUser.Font.Name, 8, FontStyle.Underline);
                btnUser.ForeColor = SystemColors.ControlText; lblUserIcon.ForeColor = SystemColors.ControlText;
            }
            else if (command == "leave")
            {
                btnUser.BackColor = SystemColors.Window; btnUser.Font = new Font(btnUser.Font.Name, 8);
                btnUser.ForeColor = SystemColors.GrayText; lblUserIcon.ForeColor = SystemColors.GrayText;
            }
        }
        //Enters the user profile page by either clicking on the button or the icon
        private void UserPageClick(object sender, EventArgs e)
        {
            EnterUserPage("");
        }
        private void UserIconClick(object sender, EventArgs e)
        {
            EnterUserPage("");
        }
        //When user either logs in or completes registration or the btnUser is clicked
        private void EnterUserPage(string username)
        {
            Text = "Blocket - Sveriges största marknadsplats";
            pnlUserPage.Visible = true; user.SignedIn = true;
            pnlTopPageButtons.Visible = true; cmbCategoryTopPagePnl.Visible = false;
            btnLogIn.Visible = false; lblLogInUserIcon.Visible = false;
            btnUser.Visible = true; lblUserIcon.Visible = true;

            if (username != string.Empty)
            {              
                lblUserName.Text = username;
                if (username.Length < 8)
                {
                    btnUser.Text = username;
                }
                else
                {
                    btnUser.Text = username.Remove(6) + "..";
                }
            }                      
            pnlSearch.Visible = false; pnlSearchResults.Visible = false;
            pnlAddAd.Visible = false; pnlNotSignedIn.Visible = false;
            pnlLogIn.Visible = false; pnlNoAds.Visible = false;
        }      

        #endregion

        #region View 2.3 - Add ad

        //Changes color of the button as its hovered or left
        private void btnAddAdHover(object sender, EventArgs e)
        {
            Image image = new Bitmap(@"C:\Users\Max\Programmering\Nackademin - Programutveckling .NET\3. Databasteknik\Inlämningsuppgifter\1. App för annonser\images\HoverAddAdButton.jpg");
            btnAddAdTopPagePnl.BackgroundImage = image;
        }
        private void btnAddAdLeave(object sender, EventArgs e)
        {
            Image image = new Bitmap(@"C:\Users\Max\Programmering\Nackademin - Programutveckling .NET\3. Databasteknik\Inlämningsuppgifter\1. App för annonser\images\AddAdButton4.0.jpg");
            btnAddAdTopPagePnl.BackgroundImage = image;
        }
        //Launches either the pnlAddAd Panel or pnlNotSignedIn Panel 
        private void AddAdTopPageClick(object sender, EventArgs e)
        {
            EnterAddAdPage("new");
        }
        private void EnterAddAdPage(string key)
        {
            pnlTopPageButtons.Visible = false;
            //Launches the add ad page if user is signed in
            if (user.SignedIn == true)
            {
                Text = "Lägg in annons";
                pnlAddAd.Visible = true;
                //Places current logged in user's username to this textbox and locks it
                txtNameAddAd.Text = lblUserName.Text;
                if (key == "update")
                {
                    lblAdId.Visible = true;
                    lblAddAdPnlTitle.Text = "Uppdatera annons på Blocket       #";
                }
                else if (key == "new")
                {
                    lblAdId.Text = string.Empty;  lblAdId.Visible = false;
                    lblAddAdPnlTitle.Text = "Lägg in annons på Blocket";                   
                }                                                   
            }
            else if (user.SignedIn == false)
            {
                Text = "Blocket - Sveriges största marknadsplats";
                pnlNotSignedIn.Visible = true;
            }
            pnlSearch.Visible = false; pnlSearchResults.Visible = false;
            pnlNoResults.Visible = false; pnlLogIn.Visible = false;
            pnlUserPage.Visible = false; pnlMyAds.Visible = false;
            pnlNoAds.Visible = false;
        }       

        #endregion

        #endregion

        #region View 3 - Username/Password/Registration page (pnlLogIn)

        #region View 3.1 - Username

        //Removes the pre-entered text in the username textbox and changes the color of the text in the textbox back to gray
        //when clicked IF the label text is red AND the textbox is empty OR is filled with the pre-entered text
        private void UsernameClick(object sender, EventArgs e)
        {
            if (lblObligatoryMsg.ForeColor == Color.Red && txtUsername.Text == string.Empty || txtUsername.Text == "👤 Ange ditt användarnamn *")
            {
                txtUsername.Text = string.Empty; txtUsername.ForeColor = Color.Gray;
            }
        }
        //Removes the pre-entered text in the username textbox when entered
        private void UsernameEnter(object sender, EventArgs e)
        {
            if (txtUsername.Text == "👤 Ange ditt användarnamn *")
            {
                txtUsername.Text = string.Empty;
            }           
        }
        //Exact opposite of above
        private void UsernameLeave(object sender, EventArgs e)
        {
            if (txtUsername.Text == string.Empty)
            {
                txtUsername.Text = "👤 Ange ditt användarnamn *";
            }
        }
        //If the ENTER-key is pressed whilst the username textbox is focused AND the text is NOT "Ange ditt använda..."
        //AND the amount of characters entered in the textbox is more or equal to 6 - the ("Fortsätt"/"Logga in" - btnContinue) is clicked
        private void UsernameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtUsername.Text != "👤 Ange ditt användarnamn *" && txtUsername.Text.Length >= 6)
            {
                btnContinue.PerformClick();
            }
        }
        //Controls the label message, colour of (label text + text in textbox)
        //and enables/disables the ("Fortsätt"/"Logga in" - btnContinue) button
        //depending on how many characters are entered
        private void UsernameKeyUp(object sender, KeyEventArgs e)
        {
            if (txtUsername.Text.Length >= 6)
            {
                btnContinue.BackColor = Color.FromArgb(65, 130, 195); btnContinue.Enabled = true;
                lblObligatoryMsg.ForeColor = Color.Gray; lblObligatoryMsg.Text = "*Obligatoriskt fält";
            }
            else if (txtUsername.Text.Length >= 1 && txtUsername.Text.Length < 6)
            {
                btnContinue.BackColor = Color.Gray; btnContinue.Enabled = false;
                lblObligatoryMsg.ForeColor = Color.Red; lblObligatoryMsg.Text = "Ange ett användarnamn mellan 6–30 tecken.";
            }
            else
            {
                btnContinue.BackColor = Color.Gray; btnContinue.Enabled = false;
                lblObligatoryMsg.ForeColor = Color.Red; lblObligatoryMsg.Text = "Fyll i ditt användarnamn här.";
            }
        }
        //Gives the user a red coloured guideline text in the textbox and label 
        public void UsernameInputError()
        {
            txtUsername.ForeColor = Color.Red; txtUsername.Text = "👤 Ange ditt användarnamn *";
            lblObligatoryMsg.ForeColor = Color.Red; lblObligatoryMsg.Text = "Fyll i ditt användarnamn här.";
        }
        //If the color of the text in the username textbox is changed and the if statement is true
        //the text will switch back to Gray and the "Fortsätt"/"Logga in"-button will be disabled (looks better) 
        private void UsernameColorChanged(object sender, EventArgs e)
        {
            if (txtUsername.ForeColor == Color.Red && lblObligatoryMsg.ForeColor == Color.Red && txtUsername.Text == string.Empty || txtUsername.Text == "👤 Ange ditt användarnamn *")
            {
                btnContinue.BackColor = Color.Gray; btnContinue.Enabled = false;
            }
        }

        #endregion

        #region View 3.2 - Password

        //Removes the pre-entered text in the password textbox and changes the color of the text in the textbox back to gray
        //when clicked IF the label text is red AND the textbox is empty OR is filled with the pre-entered text
        private void PasswordClick(object sender, EventArgs e)
        {
            if (lblObligatoryMsg.ForeColor == Color.Red && txtPassword.Text == string.Empty || txtPassword.Text == "Ange ditt lösenord *")
            {
                txtPassword.Text = string.Empty; txtPassword.ForeColor = Color.Gray;
            }
        }
        //Removes the pre-entered text in the password textbox when entered
        private void PasswordEnter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Ange ditt lösenord *")
            {
                txtPassword.Text = string.Empty; txtPassword.PasswordChar = '·';
            }
        }
        //Exact opposite of above
        private void PasswordLeave(object sender, EventArgs e)
        {
            if (txtPassword.Text == string.Empty)
            {
                txtPassword.Text = "Ange ditt lösenord *"; txtPassword.PasswordChar = '\0';
            }
        }
        //If the ENTER-key is pressed the ("Fortsätt"/"Logga in" - btnContinue) is clicked if conditions match
        private void PasswordKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtPassword.Text != "Ange ditt lösenord *" && txtPassword.Text.Length >= 3)
            {
                btnContinue.PerformClick();
            }
        }
        //Controls the label message, colour of (label text + text in textbox) and
        //enables/disables the ("Fortsätt"/"Logga in" - btnContinue) button depending on how many characters are entered
        private void PasswordKeyUp(object sender, KeyEventArgs e)
        {
            if (txtPassword.Text.Length >= 3)
            {
                btnContinue.BackColor = Color.FromArgb(65, 130, 195); lblObligatoryMsg.ForeColor = Color.Gray;
                btnContinue.Enabled = true; lblObligatoryMsg.Text = "*Obligatoriskt fält";
            }
            else if (txtPassword.Text.Length >= 1 && txtPassword.Text.Length < 3)
            {
                btnContinue.BackColor = Color.Gray; lblObligatoryMsg.ForeColor = Color.Red;
                btnContinue.Enabled = false; lblObligatoryMsg.Text = "Ditt lösenord måste innehålla minst 3 tecken.";
            }
            else
            {
                btnContinue.BackColor = Color.Gray; lblObligatoryMsg.ForeColor = Color.Red;
                btnContinue.Enabled = false; lblObligatoryMsg.Text = "Fyll i ditt lösenord här.";
            }
        }
        //Gives the user a red coloured guideline text in the textbox and label 
        public void PasswordInputError()
        {
            txtPassword.ForeColor = Color.Red; lblObligatoryMsg.ForeColor = Color.Red;
            txtPassword.Text = "Ange ditt lösenord *"; lblObligatoryMsg.Text = "Fyll i ditt lösenord här.";
        }
        //If the color of the text in the password textbox is changed and the if statement is true
        //the text will switch back to Gray and the "Fortsätt"/"Logga in"-button will be disabled (looks better) 
        private void PasswordColorChanged(object sender, EventArgs e)
        {
            if (txtPassword.ForeColor == Color.Red && lblObligatoryMsg.ForeColor == Color.Red && txtPassword.Text == string.Empty || txtPassword.Text == "Ange ditt lösenord *")
            {
                btnContinue.BackColor = Color.Gray; btnContinue.Enabled = false;
            }
        }
        //When the password textbox is visible- lblseepassword is enabled
        private void TextBoxPasswordVisible(object sender, EventArgs e)
        {
            lblSeePassword.Visible = true; lblSeePassword.Enabled = true;
        }
        //Lets the user switch between normal or * passwordChars
        private void EnableOrDisablePasswordChars(object sender, EventArgs e)
        {
            if (txtPassword.PasswordChar == '\0')
            {
                lblSeePassword.ForeColor = Color.Gray; txtPassword.PasswordChar = '·';
            }
            else
            {
                lblSeePassword.ForeColor = Color.Black; txtPassword.PasswordChar = '\0';
            }
        }

        #endregion

        #region View 3.3 - Registration 

        //Changes the look of the (login panel) to user registration mode
        private void SetRegistrationPanel()
        {
            lblLogInPnlTitle.Text = "Skapa konto"; btnContinue.Text = "Skapa konto";
            txtPassword.Visible = true; lblTermsRegistrationMsg.Visible = true;
            txtPassword.ForeColor = Color.Gray; lblObligatoryMsg.ForeColor = Color.Gray;
            lblObligatoryMsg.Location = new Point(507, 120);
        }       

        #endregion

        #region View 3.4 - Panel (combined textboxes) 

        //Changes the look of the (login panel) to user login mode 
        private void SetLogInPanel()
        {          
            lblLogInPnlTitle.Text = "Välkommen tillbaka!"; btnContinue.Text = "Logga in";
            txtPassword.Visible = true; lblObligatoryMsg.Location = new Point(507, 120);                  
        }
        //Focuses on the site title label when the continue button is enabled/disabled IF the inserted username is shorter than 6 characters
        private void ContinueEnabledChanged(object sender, EventArgs e)
        {
            if (txtConditionSearchPnl.Text.Length < 6)
            {
                lblTopPagePnlTitle.Focus();
            }
        }
        //Resets the text in the textboxes when clicking on the panel
        private void PanelClick(object sender, EventArgs e)
        {
            //Prevents exception
            if (MainForm.ActiveForm == MainForm.ActiveForm)
            {
                ResetMainFormOpacity();
            }
            if (txtUsername.Text == string.Empty)
            {
                if (lblObligatoryMsg.ForeColor == Color.Red)
                {
                    txtUsername.ForeColor = Color.Red;
                    txtUsername.Text = "👤 Ange ditt användarnamn *";  lblTopPagePnlTitle.Focus();
                }
                else
                {
                    txtUsername.Text = "👤 Ange ditt användarnamn *"; lblTopPagePnlTitle.Focus();
                }
            }
            if (txtPassword.Text == string.Empty)
            {
                //If the text is set to PasswordChar, it switches back to regular letters
                if (txtPassword.PasswordChar == '·')
                {
                    txtPassword.PasswordChar = '\0';
                }
                if (lblObligatoryMsg.ForeColor == Color.Red)
                {
                    txtPassword.ForeColor = Color.Red;
                    txtPassword.Text = "Ange ditt lösenord *"; lblTopPagePnlTitle.Focus();
                }
                else
                {
                    txtPassword.Text = "Ange ditt lösenord *"; lblTopPagePnlTitle.Focus();
                }
            }
        }

        #endregion

        #endregion

        #region View 4 - Wrong password pop-up (WrongPasswordForm)

        //To close the pop-up form (WrongPasswordForm)
        private void MainFormClick(object sender, EventArgs e)
        {
            //Prevents exception
            if (MainForm.ActiveForm == MainForm.ActiveForm)
            {
                ResetMainFormOpacity();
            }
        }
        private void pnlTopTageClick(object sender, EventArgs e)
        {
            //Prevents exception
            if (MainForm.ActiveForm == MainForm.ActiveForm)
            {
                ResetMainFormOpacity();
            }
        }
        //Resets the opacity of the MainForm 
        public void ResetMainFormOpacity()
        {
            WrongPasswordForm wrongPasswordForm = (WrongPasswordForm)Application.OpenForms["WrongPasswordForm"];

            //Prevents exception
            if (wrongPasswordForm != null)
            {
                wrongPasswordForm.Close(); lblTopPagePnlTitle.Focus();
                this.Opacity = 100;
            }
        }

        #endregion

        #region View 5 - Not signed in page (pnlNotSignedIn)

        //Mouse hover and leave switches background of button
        private void btnNotSignedInHover(object sender, EventArgs e)
        {
            Image image = new Bitmap(@"C:\Users\Max\Programmering\Nackademin - Programutveckling .NET\3. Databasteknik\Inlämningsuppgifter\1. App för annonser\images\HoverLoginGetStartedButton.jpg");
            btnNotSignedIn.BackgroundImage = image;
        }
        private void btnNotSignedInLeave(object sender, EventArgs e)
        {
            Image image = new Bitmap(@"C:\Users\Max\Programmering\Nackademin - Programutveckling .NET\3. Databasteknik\Inlämningsuppgifter\1. App för annonser\images\LoginGetStartedButton2.0.jpg");
            btnNotSignedIn.BackgroundImage = image;
        }
        //Launches the panel for login/registration
        private void LaunchPnlLogin(object sender, EventArgs e)
        {
            pnlLogIn.Visible = true; pnlTopPageButtons.Visible = false;
            pnlSearch.Visible = false; pnlSearchResults.Visible = false;
            pnlNotSignedIn.Visible = false; pnlAddAd.Visible = false;
            pnlNotSignedIn.Visible = false; pnlUserPage.Visible = false;            
        }

        #endregion

        #region View 6 - Add ad/Update ad page (pnlAddAd)

        //Makes the panel borders behave like a textbox when entered and back to regular when left
        private void CategoryEnter(object sender, EventArgs e)
        {
            pnlCategoryAddAd.BackColor = SystemColors.Highlight;
        }
        private void CategoryLeave(object sender, EventArgs e)
        {
            pnlCategoryAddAd.BackColor = SystemColors.WindowFrame;
        }
        private void PriceEnter(object sender, EventArgs e)
        {
            pnlOuterPrice.BackColor = SystemColors.Highlight;
        }
        private void PriceLeave(object sender, EventArgs e)
        {
            pnlOuterPrice.BackColor = SystemColors.WindowFrame;
        }
        //Method that resets the controls/colors
        private void PriceReset()
        {
            lblPriceWrongInput.Visible = false; 
            txtPriceAddAd.BackColor = SystemColors.Window; //Textbox to the lefT
            lblCurrencyAddAd.ForeColor = SystemColors.ControlText; //Panel to the right
            pnlInnerCurrency.BackColor = SystemColors.ControlLight;
            pnlOuterCurrency.BackColor = SystemColors.WindowFrame;
        }
        //Disables the user to enter other characters than numbers into the phone/price textbox        
        private void PriceKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                lblPriceWrongInput.Visible = true;
                //Textbox to the left
                txtPriceAddAd.BackColor = Color.FromArgb(255, 230, 230); pnlOuterPrice.BackColor = Color.Red;
                //Panel to the right
                lblCurrencyAddAd.ForeColor = Color.Red; pnlInnerCurrency.BackColor = Color.FromArgb(255, 230, 230);
                pnlOuterCurrency.BackColor = Color.Red; 
            }
            else
            {
                PriceReset();
            }
        }
        //Changes visibility of the label to false when checkBox is changed
        private void chkTermsCheckedChanged(object sender, EventArgs e)
        {
            lblTermsAddAdMsg.Visible = false;
        }
        //Removes focus of the focused control when the (pnlAddAd) is clicked 
        private void pnlAddAdClick(object sender, EventArgs e)
        {
            lblTopPagePnlTitle.Focus(); PriceReset();
        }
        //If the ENTER-key is pressed, the linked button is clicked for smoother controls
        private void PreviewAdKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnPreviewAd.PerformClick();
            }
        }
        private void UploadAdKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAddAdPnl.PerformClick();
            }
        }

        #endregion

        #region View 7 - User page/My ads/pnlNoAds (pnlUserPage & pnlNoAds)

        #region View 7.1 - User page

        private void EnterMyAds(object sender, EventArgs e)
        {
            pnlUserPage.Visible = false; DisplayMyAds();
        }      
        private void LogOutHoverEnter(object sender, EventArgs e)
        {          
            lblLogOut.Font = new Font(lblLogOut.Font.Name, 11, FontStyle.Underline);
        }
        private void LogOutHoverLeave(object sender, EventArgs e)
        {
            lblLogOut.Font = new Font(lblLogOut.Font.Name, 11);
        }       
        private void LogOutClick(object sender, EventArgs e)
        {
            user.SignedIn = false; ReloadMainPage();
        }

        #endregion

        #region View 7.2 - My ads

        private void MyAdsCellHoverMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                dgvMyAds.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new Font("Calibri", 14, FontStyle.Underline);
            }
        }
        private void MyAdsCellHoverLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                dgvMyAds.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new Font("Calibri", 14, FontStyle.Regular);
            }
        }
        //Shows ads of the current logged in user      
        private void DisplayMyAds()
        {
            pnlMyAds.Visible = true;
            DataTable returnValues = ad.DisplayMyAds(lblUserName.Text);
            dgvMyAds.DataSource = returnValues;
            if (returnValues.Rows.Count < 1)
            {
                pnlNoAds.Visible = true;
            }
            else
            {
                dgvMyAds.Sort(dgvMyAds.Columns["DateAdded"], ListSortDirection.Descending);
            }
        }

        #endregion

        #region View 7.3 - pnlNoAds

        private void btnNoAdHover(object sender, EventArgs e)
        {
            Image image = new Bitmap(@"C:\Users\Max\Programmering\Nackademin - Programutveckling .NET\3. Databasteknik\Inlämningsuppgifter\1. App för annonser\images\HoverNoAdButton.jpg");
            btnNoAd.BackgroundImage = image;
        }
        private void btnNoAdLeave(object sender, EventArgs e)
        {
            Image image = new Bitmap(@"C:\Users\Max\Programmering\Nackademin - Programutveckling .NET\3. Databasteknik\Inlämningsuppgifter\1. App för annonser\images\btnNoAd.jpg");
            btnNoAd.BackgroundImage = image;
        }
        private void AddAdNoAdClick(object sender, EventArgs e)
        {
            EnterAddAdPage("new");
        }

        #endregion

        #endregion

        #region View 8 - View ad (pnlViewAd)

        private void FillAdInfo(List<DataRow> retrievedAd, string key)
        {
            if (key == "update" || key == "view")
            {
                List<string> addDetail = manager.OutputChecker(retrievedAd, "full");
                if (key == "update")
                {
                    txtPhoneAddAd.Text = addDetail[0];
                    cmbCategoryAddAd.Text = addDetail[1];
                    txtTitleAddAd.Text = addDetail[2];
                    txtTextAddAd.Text = addDetail[3];
                    txtPriceAddAd.Text = addDetail[4];
                }
                else if (key == "view")
                {
                    lblAdCategory.Text = addDetail[1];
                    lblAdTitle.Text = addDetail[2];
                    lblAdPrice.Text = addDetail[4].TrimStart() + " kr";
                    lblAdUsername.Text = addDetail[5];
                    btnShowPhone.Text = "📞 " + addDetail[0];
                    txtAdDescription.Text = addDetail[3];
                }
            }
            else
            {
                List<string> addDetail = manager.OutputChecker(retrievedAd, "half");
                lblAdUsername.Text = addDetail[2];
                btnShowPhone.Text = "📞 " + addDetail[0];
                txtAdDescription.Text = addDetail[1];
            }
        }
        private void ShowPhoneClick(object sender, EventArgs e)
        {
            Image image = new Bitmap(@"C:\Users\Max\Programmering\Nackademin - Programutveckling .NET\3. Databasteknik\Inlämningsuppgifter\1. App för annonser\images\btnShowPhone2.0.jpg");
            btnShowPhone.BackgroundImage = image;

            btnShowPhone.Font = new Font(btnShowPhone.Font.Name, 12, FontStyle.Bold);
        }
        private void EnterViewPanel()
        {
            pnlViewAd.Visible = true;
            pnlViewAd.BringToFront();
        }
        private void ResetViewPanel()
        {
            lblAdCategory.Text = string.Empty; lblAdDateAdded.Text = "🕘 Inlagd:";
            lblAdTitle.Text = string.Empty; lblAdPrice.Text = string.Empty;
            lblAdUsername.Text = string.Empty; btnShowPhone.Text = string.Empty;
            txtAdDescription.Text = string.Empty; btnShowPhone.Font = new Font(btnShowPhone.Font.Name, 1, FontStyle.Regular);
            Image image = new Bitmap(@"C:\Users\Max\Programmering\Nackademin - Programutveckling .NET\3. Databasteknik\Inlämningsuppgifter\1. App för annonser\images\btnShowPhone.jpg");
            btnShowPhone.BackgroundImage = image;

            pnlViewAd.Visible = false; pnlViewAd.SendToBack();
        }
        private void CloseView(object sender, EventArgs e)
        {
            ResetViewPanel();
        }

        #endregion

        #region Misc. controls

        //Allows the user to reach back to the main page when the title is clicked
        private void TopTageTitleClick(object sender, EventArgs e)
        {
            ReloadMainPage();
        }
        private void ReloadMainPage()
        {
            Text = "Blocket - köp & sälj bilar, möbler, lägenheter, cyklar och mer";
            pnlSearch.Visible = true; pnlTopPageButtons.Visible = true;
            cmbCategoryTopPagePnl.Visible = true;
            if (user.SignedIn == true)
            {
                btnUser.Visible = true; lblUserIcon.Visible = true;
                btnLogIn.Visible = false; lblLogInUserIcon.Visible = false;
                lblAdId.Text = string.Empty;
            }
            else if (user.SignedIn == false)
            {
                btnLogIn.Visible = true; lblLogInUserIcon.Visible = true;
                btnUser.Visible = false; lblUserIcon.Visible = false;
                ResetLogInPage();
            }
            pnlSearchResults.Visible = false; pnlNoResults.Visible = false;
            pnlAddAd.Visible = false; pnlNotSignedIn.Visible = false;
            pnlLogIn.Visible = false; pnlUserPage.Visible = false;           
            pnlAddAd.Visible = false; pnlMyAds.Visible = false;
            pnlNoAds.Visible = false;
        }

        private void ResetLogInPage()
        {
            lblLogInPnlTitle.Text = "Logga in eller skapa konto";
            txtUsername.Text = "👤 Ange ditt användarnamn *";
            lblObligatoryMsg.Location = new Point(506, 80);
            txtPassword.Visible = false;
            btnContinue.Text = "Fortsätt";
        }

        //Displays a MessageBox with the user terms and agreements
        private void lblUserTermsClick(object sender, EventArgs e)
        {
            string message = "För att alla ska kunna handla tryggt och säkert på Blocket behöver\n" +
                             "du följa våra användarvillkor. Läs igenom villkoren och kontakta \n" +
                             "kundservice om det är något du är osäker över. \n" +
                             " \n" +
                             "Villkoren är till för din säkerhet på Blocket \n" +
                             "• Annonser som inte följer reglerna publiceras inte \n" +
                             "• Radera eller förläng din gamla annons om du ska annonsera samma \n" +
                             "  vara eller tjänst igen \n" +
                             "• Är du osäker på om din annons följer reglerna, fråga kundservice \n" +
                             " \n" +
                             "Beskrivning \n" +
                             "Annonsrubriken måste beskriva varan eller tjänsten, inga företagsnamn " +
                             "eller länkningar får förekomma. Inga onödiga tecken får användas i " +
                             "rubriken. Varan eller tjänsten måste beskrivas i annonstexten, det är " +
                             "inte tillåtet att endast länka till en annan sida. Annonstexter får inte " +
                             "kopieras från andra annonser, dessa kan vara skyddade av upphovsrätt " +
                             "och / eller andra lagar och är Blockets egendom enligt vad som anges " +
                             "ovan. Det är inte tillåtet att använda sig av sådana sökord i " +
                             "annonstexten som gör att användare får felaktiga annonsträffar eller " +
                             "som innebär otillåten användning av annans varumärke.";

            MessageBox.Show(message, "Användarvillkor på Blocket", MessageBoxButtons.OK);
        }

        #endregion

        #endregion  
    }
}
