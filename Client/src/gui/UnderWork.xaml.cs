
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    public partial class UnderWork : UserControl
    {
        public UnderWork()
        {
            InitializeComponent();
        }

        private void Go_Back(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenFileCreationPage();
        }
    }
}
