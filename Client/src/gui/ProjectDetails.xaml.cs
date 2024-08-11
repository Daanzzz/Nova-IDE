using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Client.GUI_Files;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Client
{
    
    public partial class ProjectDetails : UserControl
    {
        string originalText;
        string folderPath;
        string filesPath;
        string projectName;
        public ProjectDetails()
        {
            InitializeComponent();
            originalText = textInput.Text; // Store the original text
        }


        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenMenuPage();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).OpenFileCreationPage();
            PathUi.Text = "";
            textInput.Text = "New Project";
        }

        private void SelectDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select a Directory";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedDirectory = dialog.FileName;
                this.folderPath = selectedDirectory;
                PathUi.Text = selectedDirectory;
            }


        }

        private bool checkIfAProjectExists() 
        {
            string[] files = Directory.GetFiles(PathUi.Text);
            foreach (string file in files)
            {
                if(file.EndsWith(".nova"))
                {
                    return true;
                }
            }

            return false;
        }
        private void done(object sender, RoutedEventArgs e)
        {
            if(PathUi.Text.Length == 0)
            {
                MessageBox.Show("Path cannot be empty!");
                return;
            }

            this.projectName = textInput.Text;
            try
            {
                this.filesPath = this.folderPath + "\\" + this.projectName;
                // Check if the folder exists, if not create it
                if (!Directory.Exists(this.filesPath))
                {
                    Directory.CreateDirectory(this.filesPath);
                }

                string fileName = this.projectName + ".cpp";
                string projectFileName = this.projectName + ".nova";

                // Combine the folder path and file name
                string filePath = Path.Combine(this.filesPath, fileName);
                string newProjectPath = Path.Combine(this.folderPath, projectFileName);

                // Check if the file already exists
                if (File.Exists(filePath))
                {
                    MessageBox.Show("File already exists!");
                }
                else
                {
                    File.Create(filePath);
                    File.Create(newProjectPath);
                    //MessageBox.Show("File created successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            (Window.GetWindow(this) as MainWindow).updateProjectDetails(this.folderPath, this.filesPath, this.projectName);
            (Window.GetWindow(this) as MainWindow).OpenIdePage(false, "");
        }
    }
}
