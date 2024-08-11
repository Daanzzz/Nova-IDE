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
    /// Interaction logic for DeleteFile.xaml
    /// </summary>
    public partial class DeleteFile : UserControl
    {
        public DeleteFile()
        {
            InitializeComponent();
        }
        private void done(object sender, RoutedEventArgs e)
        {
            if (filename.Text == string.Empty)
            {
                MessageBox.Show("File name cant be empty!");
            }
            else if (!filename.Text.EndsWith(".cpp") && !filename.Text.EndsWith(".h")) // change this to button press
            {
                MessageBox.Show("File type isn't correct");
            }
            else
            {
                (Window.GetWindow(this) as MainWindow).removeFile(filename.Text);
                (Window.GetWindow(this) as MainWindow).OpenIdePage(false, "");
                filename.Text = string.Empty;
            }

        }
    }
}
