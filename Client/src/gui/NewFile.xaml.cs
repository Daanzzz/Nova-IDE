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
    /// Interaction logic for NewFile.xaml
    /// </summary>
    public partial class NewFile : UserControl
    {
        public NewFile()
        {
            InitializeComponent();
        }
        private void done(object sender, RoutedEventArgs e)
        {
            if(filename.Text == string.Empty)
            {
                MessageBox.Show("File name cant be empty!");
            }
            else
            {
                string selectedExtension = (extensionComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                string fileName = filename.Text + selectedExtension;
                (Window.GetWindow(this) as MainWindow).updateNewFileName(fileName);
                (Window.GetWindow(this) as MainWindow).OpenIdePage(false, "");
                filename.Text = string.Empty;
            }
            
        }
    }
}
