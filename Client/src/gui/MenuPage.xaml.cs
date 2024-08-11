
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public partial class MenuPage : UserControl
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        // need 2 refarctor
        private void New_Project(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.Effect = new System.Windows.Media.Effects.DropShadowEffect()
            {
                BlurRadius = 10,
                ShadowDepth = 5
            };
            Thread.Sleep(500);
            (Window.GetWindow(this) as MainWindow).OpenFileCreationPage();
        }
        
        // need 2 refactor
        private void Open_Folder(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
