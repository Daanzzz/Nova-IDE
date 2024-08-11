using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServerCommunicatorNS;

namespace Client.src.gui
{
    /// <summary>
    /// Interaction logic for Signup.xaml
    /// </summary>
    public partial class Signup : UserControl
    {

        private const string INFO_CORRECT = "9";
        private const string INCORRECT_PASSWORD = "1";
        private const string INVALID_USER = "2";
        private const string USER_ALREADY_EXISTS = "3";
        private const string EMAIL_ALREADY_EXISTS = "4";


        private string realPassword;
        private ServerCommunicator sc;
        public Signup(ref ServerCommunicator sc)
        {
            InitializeComponent();
            realPassword = "";
            this.sc = sc;
        }

        private void done(object sender, RoutedEventArgs e)
        {
            bool done = true;

            bool passwordCheck = true;

            //Username valididy checks
            if (username.Text.Length < 3)
            {
                MessageBox.Show("Username should be at least 3 characters long");
                done = false;
            }
            if (username.Text.Length > 8)
            {
                MessageBox.Show("Username can't be longer than 8 characters");
                done = false;
            }


            if (password.Text.Length < 8)
            {
                MessageBox.Show("Password should be at least 8 characters long");
                done = false;
                passwordCheck = false;
            }

            if (!password.Text.Any(char.IsUpper))
            {
                MessageBox.Show("Password should contain at least one uppercase letter");
                done = false;
                passwordCheck = false;
            }

            if (!password.Text.Any(char.IsLower))
            {
                MessageBox.Show("Password should contain at least one lowercase letter");
                done = false;
                passwordCheck = false;
            }

            if (!password.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Password should contain at least one digit");
                done = false;
                passwordCheck = false;
            }

            //Email valididy checks
            if (!email.Text.Contains('@') || !email.Text.Contains('.'))
            {
                MessageBox.Show("Email invalid");
                done = false;
            }



            if (done)
            {
                string message = sc.sendSignup(username.Text, password.Text, email.Text);
                string code = message[0].ToString();
                if (code ==  USER_ALREADY_EXISTS)
                {
                    MessageBox.Show("Username already exists");
                }
                else if (code ==  EMAIL_ALREADY_EXISTS)
                {
                    MessageBox.Show("Email already exists");
                }
                else if (code == INFO_CORRECT)
                {
                    (Window.GetWindow(this) as MainWindow).updateAction("signup");
                    (Window.GetWindow(this) as MainWindow).updateEmail(email.Text);
                    (Window.GetWindow(this) as MainWindow).updateUsernameVer(username.Text);
                    (Window.GetWindow(this) as MainWindow).updatePasswordVer(password.Text);
                    (Window.GetWindow(this) as MainWindow).OpenVerificationPage();
                    username.Text = "";
                    password.Text = "";
                    email.Text = "";
                }


                
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenFirstPage();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenFirstPage();
        }

        public bool IsAnimationEnabled
        {
            get { return (bool)GetValue(IsAnimationEnabledProperty); }
            set { SetValue(IsAnimationEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsAnimationEnabledProperty =
            DependencyProperty.Register("IsAnimationEnabled", typeof(bool), typeof(Signup), new PropertyMetadata(true));

        private void password_TextChanged(object sender, TextChangedEventArgs e)
        {
            

        }

    }
}