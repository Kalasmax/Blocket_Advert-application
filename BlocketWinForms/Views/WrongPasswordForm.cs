using System;
using System.Drawing;
using System.Windows.Forms;

namespace BlocketWinForms.Views
{
    public partial class WrongPasswordForm : System.Windows.Forms.Form
    {
        MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];

        public WrongPasswordForm()
        {
            InitializeComponent();
        }

        //Changes color of the button as its hovered or left
        private void btnOKHover(object sender, EventArgs e)
        {
            Image image = new Bitmap(@"C:\Users\Max\Programmering\Nackademin - Programutveckling .NET\3. Databasteknik\Inlämningsuppgifter\1. App för annonser\HoverOKButton.jpg");
            btnOK.BackgroundImage = image;
        }
        private void btnOKLeave(object sender, EventArgs e)
        {
            Image image = new Bitmap(@"C:\Users\Max\Programmering\Nackademin - Programutveckling .NET\3. Databasteknik\Inlämningsuppgifter\1. App för annonser\OKButton.jpg");
            btnOK.BackgroundImage = image;
        }

        //Close the messagebox form anywhere the user is clicking
        private void btnOKClick(object sender, EventArgs e)
        {
            mainForm.ResetMainFormOpacity();          
        }
        private void WrongPasswordFormClick(object sender, EventArgs e)
        {
            mainForm.ResetMainFormOpacity();          
        }  
    }
}
