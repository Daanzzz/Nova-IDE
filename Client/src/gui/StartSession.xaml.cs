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
using P2PCommunicatorNS;

namespace Client.src.gui
{
    /// <summary>
    /// Interaction logic for StartSession.xaml
    /// </summary>
    public partial class StartSession : UserControl
    {
        private const string INVALID_USER = "2";

        private ServerCommunicator sc;
        private P2PCommunicator p2p;
        public StartSession(ref ServerCommunicator sc, ref P2PCommunicator p2p)
        {
            InitializeComponent();
            this.sc = sc;
            this.p2p = p2p;
        }

        private void done(object sender, RoutedEventArgs e)
        {
            if(ui_username.Text != string.Empty)
            {
                string message = sc.sendInvite(ui_username.Text);
                //MessageBox.Show(message);
                if (message[0].ToString() == INVALID_USER)
                {
                    MessageBox.Show("Username doesn't exist");
                }
                else
                {
                    //(Window.GetWindow(this) as MainWindow).connectP2P(message);
                    this.p2p.SetUDPClient(ref sc.getSoc());
                    this.p2p.ConnectP2P(message);
                    this.p2p.startSesstion();

                    (Window.GetWindow(this) as MainWindow).OpenIdePage(true, ui_username.Text);
                }
            }
            else
            {
                MessageBox.Show("Please put in a username");
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenIdePage(false, "");
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenIdePage(false, "");

        }
    }
}
