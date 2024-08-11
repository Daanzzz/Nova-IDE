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
    /// Interaction logic for NewPassword.xaml
    /// </summary>

    
    public partial class NewPassword : UserControl
    {
        private const string INFO_CORRECT = "9";

        private ServerCommunicator sc;
        private string code;
        private string username;
        private string email;
        public NewPassword(ref ServerCommunicator sc)
        {
            InitializeComponent();
            this.sc = sc;
        }

        public void setCode(string code)
        {
            this.code = code;
        }
        public void setUsername(string username)
        {
            this.username = username;
        }
        public void setEmail(string email)
        {
            this.email = email;
        }
        private void done(object sender, RoutedEventArgs e)
        {
            if (password1.Text != password2.Text)
            {
                MessageBox.Show("Passwords don't match!");
            }
            else
            {
                bool done = true;
                if (password1.Text.Length < 8)
                {
                    MessageBox.Show("Password should be at least 8 characters long");
                    done = false;
                }

                if (!password1.Text.Any(char.IsUpper))
                {
                    MessageBox.Show("Password should contain at least one uppercase letter");
                    done = false;
                }

                if (!password1.Text.Any(char.IsLower))
                {
                    MessageBox.Show("Password should contain at least one lowercase letter");
                    done = false;
                }

                if (!password1.Text.Any(char.IsDigit))
                {
                    MessageBox.Show("Password should contain at least one digit");
                    done = false;
                }

                if(done)
                {
                    string message = sc.sendNewPassword(this.username, password1.Text, this.email ,this.code);
                    if (message[0].ToString() != INFO_CORRECT)
                    {
                        MessageBox.Show("Something went wrong");
                    }
                    else
                    {
                        MessageBox.Show("Password changed successfully");
                        (Window.GetWindow(this) as MainWindow).OpenLoginPage();
                    }
                }
            }
            
        }
    }
}
