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
    /// Interaction logic for PasswordRecovery.xaml
    /// </summary>
    public partial class PasswordRecovery : UserControl
    {

        private const string EMAIL_DOESNT_EXISTS = "5";
        private const string INVALID_USER = "2";
        private const string INFO_CORRECT = "9";

        private ServerCommunicator sc;
        public PasswordRecovery(ref ServerCommunicator sc)
        {
            InitializeComponent();
            this.sc = sc;
        }
        private void done(object sender, RoutedEventArgs e)
        {
            string message = sc.checkEmail(ui_username.Text, email.Text);
            if (message[0].ToString() == EMAIL_DOESNT_EXISTS)
            {
                MessageBox.Show("Email doesn't exist");
            }
            else if (message[0].ToString() == INVALID_USER)
            {
                MessageBox.Show("User doesn't exist");
            }
            else
            {
                (Window.GetWindow(this) as MainWindow).updateUsername(ui_username.Text);
                (Window.GetWindow(this) as MainWindow).updateAction("recovery");
                (Window.GetWindow(this) as MainWindow).updateEmail(email.Text);
                (Window.GetWindow(this) as MainWindow).updateEmailInPassword(email.Text);
                (Window.GetWindow(this) as MainWindow).OpenVerificationPage();
                email.Text = "";
            }
        }


        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenFirstPage();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenLoginPage();
        }
    }
}
