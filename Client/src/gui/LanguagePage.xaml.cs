
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public partial class LanguagePage : UserControl
    {
        public LanguagePage()
        {
            InitializeComponent();
        }
        
        private void CPP_CLick(object sender, System.Windows.RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).openNewProjectPage();
        }
        private void C_CLick(object sender, System.Windows.RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).openUnderWorkPage();
        }

  
        private void Py_CLick(object sender, System.Windows.RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).openUnderWorkPage();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenMenuPage();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenMenuPage();
        }
    }
}
