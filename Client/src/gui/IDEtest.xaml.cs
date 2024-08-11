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

namespace Client.src.gui
{
    /// <summary>
    /// Interaction logic for IDEtest.xaml
    /// </summary>
    public partial class IDEtest : UserControl
    {
        public IDEtest()
        {
            InitializeComponent();
            Loaded += Ide_Loaded;
        }
        private void Ide_Loaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is Window mainWindow)
            {
                // Set the desired screen resolution
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;

                //mainWindow.Width = screenWidth;
                mainWindow.WindowState = WindowState.Minimized;
                mainWindow.Height = screenHeight - 100;
                mainWindow.WindowStyle = WindowStyle.None; // Optionally remove the window chrome
            }
        }
    }
}
