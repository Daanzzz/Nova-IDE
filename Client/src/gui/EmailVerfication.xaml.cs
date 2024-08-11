using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServerCommunicatorNS;

namespace Client.src.gui
{
    /// <summary>
    /// Interaction logic for EmailVerfication.xaml
    /// </summary>
    public partial class EmailVerfication : UserControl
    {
        private string email;
        private string action;
        private string username;
        private string password;
        private ServerCommunicator sc;

        
        private const string WRONG_CODE = "5";
        private const string INFO_CORRECT = "9";
        

        public EmailVerfication(ref ServerCommunicator sc)
        {
            InitializeComponent();
            this.sc = sc;
        }

        public void setAction(string action)
        {
            this.action = action;
        }
        public void setEmail(string email)
        {
            this.email = email;
            first.Text = "We have sent a code to: " + email;
        }
        public void setUsername(string username) 
        { 
            this.username = username;
        }

        public void setPassword(string password)
        {
            this.password = password;
        }
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenFirstPage();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (action == "signup")
            {
                (Window.GetWindow(this) as MainWindow).OpenSignupPage();
            }
            else if (action == "recovery")
            {
                (Window.GetWindow(this) as MainWindow).OpenRecoveryPage();
            }
            
        }

        private void done(object sender, RoutedEventArgs e)
        {
            string message = sc.checkCode(this.username, this.password, this.email ,codeText.Text);
            bool codeCorrect;
            if (message[0].ToString() == INFO_CORRECT)
            {
                codeCorrect = true;
            }
            else
            {
                codeCorrect = false;
            }

            if(!codeCorrect)
            {
                MessageBox.Show("Code Incorrect!");
            }
            else
            {
                if(action == "signup")
                {
                    MessageBox.Show("Signed up succrefully!");
                    (Window.GetWindow(this) as MainWindow).OpenLoginPage();
                }
                else if(action == "recovery")
                {
                    (Window.GetWindow(this) as MainWindow).updateCode(codeText.Text);
                    (Window.GetWindow(this) as MainWindow).openNewPasswordPage();
                }
            }
        }
    }
}
