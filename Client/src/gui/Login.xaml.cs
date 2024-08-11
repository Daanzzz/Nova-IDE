using System;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        private ServerCommunicator sc;
        private const string INFO_CORRECT = "9";
        private const string INCORRECT_PASSWORD = "1";
        private const string INVALID_USER = "2";
        private const string USER_ALREADY_EXISTS = "3";
        private const string EMAIL_ALREADY_EXISTS = "4";

        public Login(ref ServerCommunicator sc)
        {
            InitializeComponent();
            this.sc = sc;
        }
        private void done(object sender, RoutedEventArgs e)
        {
            if (username.Text.Length == 0 || password.Text.Length == 0)
            {
                MessageBox.Show("Fields can't be empty!");
            }
            else
            {
                string message = sc.sendLogin("Hammer", "Hammer1234");
                string code = message[0].ToString();
                if (code ==  INCORRECT_PASSWORD)
                {
                    MessageBox.Show("Password incorrect");
                    password.Text = "";
                }
                else if (code ==  INVALID_USER)
                {
                    MessageBox.Show("User doesn't exist");
                }
                else if (code == INFO_CORRECT)
                {
                    string token = "";
                    for (int i = 2; i < message.Length; i++)
                    {
                        token += message[i];
                    }
                    sc.setSessionToken(token);
                    MessageBox.Show("Logged in");
                    (Window.GetWindow(this) as MainWindow).OpenMenuPage();
                    username.Text = "";
                    password.Text = "";
                }
            }
            
        }

        private void forgotPassword(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenRecoveryPage();
            username.Text = "";
            password.Text = "";
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenFirstPage();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenFirstPage();
        }
    }
}
