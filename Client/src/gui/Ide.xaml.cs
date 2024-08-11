using P2PCommunicatorNS;
using SynchronizationManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Client.GUI_Files
{
    public partial class Ide : UserControl
    {
        public const string ADDITION_IDENTIFIER = "+";
        public const string DELETION_IDENTIFIER = "-";
        public const string MULTIPLE_ACTION_IDENTIFIER = "=";
        public const string UPDATE_IDENTIFIER = "U";

        const int ACTION = 0;
        const int LENGTH = 2;
        const int OFFSET = 4;
        const int TEXT = 6;

        //private ServerCommunicator sc;
        private P2PCommunicator P2PCM;
        private SynchronizationManager SyncManager;

        private int tokens;
        private bool deletionFlag;
        private int crosshairPlace;
        private string otherUser;
        private bool connected;

        public string projectPath;
        public string filesPath;
        private string projectName;
        //private string newFileName;

        private string currentFilePath = "";
        private string currentFileContent = "";

        private bool updatingFiles;
        private bool sessionStarted;

        public Ide(ref P2PCommunicator p2pCM, ref SynchronizationManager SyncMngr)
        {
            InitializeComponent();

            this.P2PCM = p2pCM;
            this.tokens = 0;
            this.deletionFlag = false;
            crosshairPlace = 0;

            this.SyncManager = SyncMngr;
            //this.sc = sc;

            Loaded += Ide_Loaded;
            notepad.Background = Brushes.Black;
            notepad.Foreground = Brushes.White;

            connected = false;
            sessionStarted = false;

            var ideUpdater = new Thread(textUpdater);
            ideUpdater.IsBackground = true;
            ideUpdater.Start();


            fileMenuItem.MouseEnter += FileMenuItem_MouseEnter;
            fileMenuItem.MouseLeave += FileMenuItem_MouseLeave;
            hoverTab.MouseEnter += HoverTab_MouseEnter;
            hoverTab.MouseLeave += HoverTab_MouseLeave;


        }

        private void AddFile(string fileName, Brush foregroundBrush)
        {
            // Get the parent item if it exists, otherwise create one
            TreeViewItem parentItem = fileExplorer.Items.Count > 0 ? fileExplorer.Items[0] as TreeViewItem : new TreeViewItem();

            if (fileExplorer.Items.Count == 0)
            {
                // If no parent item exists, create one and add it to the TreeView
                parentItem.Header = "Files";
                fileExplorer.Items.Add(parentItem);
            }

            // Create a new TreeViewItem for the file
            TreeViewItem fileItem = new TreeViewItem();
            fileItem.Header = fileName;
            fileItem.Foreground = foregroundBrush; // Set the foreground color

            // Add the file item to the appropriate level based on the file extension
            if (fileName.EndsWith(".cpp"))
            {
                // Check if the .cpp level exists, otherwise create one
                TreeViewItem cppItem = parentItem.Items.Count > 0 ? parentItem.Items[0] as TreeViewItem : new TreeViewItem();
                if (parentItem.Items.Count == 0)
                {
                    cppItem.Header = "CPP Files"; // Change header to "CPP Files"
                    cppItem.Foreground = Brushes.White; // Set foreground color to white
                    parentItem.Items.Add(cppItem);
                }
                cppItem.Items.Add(fileItem); // Add the file item to the .cpp level
                cppItem.IsExpanded = true; // Expand the .cpp level to show the newly added file item
            }
            else if (fileName.EndsWith(".h"))
            {
                // Check if the .h level exists, otherwise create one
                TreeViewItem hItem = parentItem.Items.Count > 1 ? parentItem.Items[1] as TreeViewItem : new TreeViewItem();
                if (parentItem.Items.Count <= 1)
                {
                    hItem.Header = "Header Files"; // Change header to "Header Files"
                    hItem.Foreground = Brushes.White; // Set foreground color to white
                    parentItem.Items.Add(hItem);
                }
                hItem.Items.Add(fileItem); // Add the file item to the .h level
                hItem.IsExpanded = true; // Expand the .h level to show the newly added file item
            }
            else
            {
                // Add the file item to the general files level
                parentItem.Items.Add(fileItem);
                parentItem.IsExpanded = true; // Expand the parent item to show the newly added file item
            }

            first.Text = fileName;
            //changeSelectionOfFileInTreeView(fileName, true, fileExplorer);
        }

        private void RemoveFileFromTreeView(string fileName)
        {
            // Find and remove the TreeViewItem corresponding to the specified filename
            foreach (TreeViewItem parentItem in fileExplorer.Items)
            {
                RemoveFileFromTreeViewRecursive(parentItem, fileName);
            }
        }

        private bool RemoveFileFromTreeViewRecursive(TreeViewItem parentItem, string fileName)
        {
            foreach (TreeViewItem item in parentItem.Items)
            {
                if (item.Header.ToString() == fileName)
                {
                    parentItem.Items.Remove(item);
                    return true; // Item found and removed
                }
                // Check recursively if the item is found in child items
                if (RemoveFileFromTreeViewRecursive(item, fileName))
                    return true;
            }
            return false; // Item not found in this subtree
        }

        private void FileExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Get the selected item from the TreeView
            TreeViewItem selectedItem = (TreeViewItem)fileExplorer.SelectedItem;

            if (selectedItem != null)
            {
                // Check if the selected item is double-clicked
                
                // Access the header (text) of the selected item
                string fileName = selectedItem.Header.ToString();
                if (fileName != "CPP Files" && fileName != "Header Files" && fileName != "Files")
                {
                    switchFile(fileName);
                }

                
            }
        }

        private void loadNewFile(string fileName)
        {
            SaveContentToFile(this.currentFilePath, notepad.Text);
            string currentFileName = Path.GetFileName(this.currentFilePath);

            this.currentFilePath = Path.Combine(this.filesPath, fileName);
            notepad.Text = string.Empty;
            first.Text = fileName;

        }

        private void switchFile(string fileName)
        {
            this.updatingFiles = true;
            // Save current file
            SaveContentToFile(this.currentFilePath, notepad.Text);


            this.currentFilePath = Path.Combine(this.filesPath, fileName);
            // Load file content into the IDE
            LoadFileIntoIDE(fileName);
            first.Text = fileName;
            this.updatingFiles = false;
        }

        private void LoadFileIntoIDE(string fileName)
        {

            // Construct the file path for the new file
            string filePath = Path.Combine(this.filesPath, fileName);

            try
            {
                // Read the content of the new file
                string fileContent = File.ReadAllText(filePath);

                // Set the IDE text to the content of the new file
                notepad.Text = fileContent;

                // Update the current file path and content
                currentFilePath = filePath;
                currentFileContent = fileContent;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading file: " + ex.Message);
            }
        }

        private void SaveContentToFile(string filePath, string content)
        {
            try
            {
                // Write the content to the file
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file: " + ex.Message);
            }
        }
        private void startSession(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).openStartSessionPage();
        }
        public void setProjectDetails(string projectPath, string filesPath, string projectName)
        {
            this.projectPath = projectPath;
            this.filesPath = filesPath;
            this.projectName = projectName;
            string fileName = projectName + ".cpp";
            AddFile(fileName, Brushes.LightGray);
            this.currentFilePath = Path.Combine(this.filesPath, fileName);
            //switchFile(fileName);


        }

        public void setNewFileName(string fileName)
        {
            //this.newFileName = fileName;
            string filePath = Path.Combine(this.filesPath, fileName);

            if (File.Exists(filePath))
            {
                MessageBox.Show("File already exists!");
            }
            else
            {
                File.Create(filePath);
                AddFile(fileName, Brushes.LightGray);
                loadNewFile(fileName);
            }
        }

        public void removeFile(string fileName)
        {
            string filePath = Path.Combine(this.filesPath, fileName);


            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    RemoveFileFromTreeView(fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting file: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("File does not exist!");
            }

            if (filePath.ToLower() == this.currentFilePath.ToLower())
            {
                string[] files = Directory.GetFiles(this.filesPath);

                // Check if there are any files
                if (files.Length > 0)
                {
                    string transferFileName = Path.GetFileName(files[0]);
                    this.currentFilePath = Path.Combine(this.filesPath, transferFileName);
                    LoadFileIntoIDE(transferFileName);
                    first.Text = transferFileName;
                }
                else
                {
                    notepad.Text = string.Empty;
                    first.Text = "No files!";
                }
            }
        }



        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Handle menu item click events here
            MenuItem menuItem = sender as MenuItem;
            if (menuItem.Header.ToString() == "New File")
            {
                (Window.GetWindow(this) as MainWindow).openNewFilePage();
            }
            if (menuItem.Header.ToString() == "Remove File")
            {
                (Window.GetWindow(this) as MainWindow).openDeleteFilePage();
            }
            else if (menuItem.Header.ToString() == "Save Files")
            {
                SaveContentToFile(this.currentFilePath, notepad.Text);
            }
        }

        private void FileMenuItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Show hover tab
            hoverTab.Visibility = Visibility.Visible;
        }

        private void FileMenuItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Hide hover tab
            hoverTab.Visibility = Visibility.Hidden;
        }

        private void HoverTab_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Show hover tab
            hoverTab.Visibility = Visibility.Visible;
        }

        private void HoverTab_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Hide hover tab
            hoverTab.Visibility = Visibility.Hidden;
        }


        public void setOtherUser(string username)
        {
            this.updatingFiles = true;
            otherUser = username;
            connected = true;

            connected_ui.Text = "Connected to:";
            otherUser_ui.Text = username;

            //notepad.Text = string.Empty;

            if (this.P2PCM.GetHost() == true)
            {
                this.P2PCM.sendFiles(buildFiles());
            }
            else
            {
                Dictionary<string, string> files = this.P2PCM.getFiles();
                recieveFiles(files);
                setConent(files);
            }
            this.updatingFiles = false;
            this.sessionStarted = true;
        }


        public Dictionary<string, string> buildFiles()
        {
            Dictionary<string, string> filesDict = new Dictionary<string, string>();
            string[] files = Directory.GetFiles(this.filesPath);
            string currentFileText = string.Empty;

            foreach (string file in files)
            {
                currentFileText = File.ReadAllText(file);
                filesDict.Add(Path.GetFileName(file), currentFileText);
            }

            return filesDict;
        }


        public void recieveFiles(Dictionary<string, string> filesDict)
        {

            //removing all current files
            string[] files = Directory.GetFiles(this.filesPath);
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                removeFile(fileName);
            }


            string currentFileName = string.Empty;
            string currentFilePath = string.Empty;
            string currentFileText = string.Empty;
            foreach (string file in filesDict.Keys)
            {
                currentFileName = Path.GetFileName(file);
                currentFilePath = Path.Combine(this.filesPath, currentFileName);

                File.Create(currentFilePath);
                AddFile(currentFileName, Brushes.LightGray);
            }
        }


        public void setConent(Dictionary<string, string> filesDict)
        {
            bool isFirst = true;

            foreach (string file in filesDict.Keys)
            {
                string filePath = Path.Combine(this.filesPath, file);
                string currentFileText = filesDict[file];
                try
                {
                    File.WriteAllText(filePath, currentFileText);

                    if (isFirst)
                    {
                        LoadFileIntoIDE(filePath);
                        isFirst = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file: " + ex.Message);
                }
            }
        }

        private void Ide_Loaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is Window mainWindow)
            {
                // Set the desired screen resolution
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tokens == 0 && this.updatingFiles == false && this.sessionStarted == true)
            {
                ICollection<TextChange> changes = e.Changes;

                foreach (TextChange change in changes)
                {
                    int offset = change.Offset;
                    int addedLength = change.AddedLength;
                    int removedLength = change.RemovedLength;

                    if (offset >= 0 && offset + addedLength <= notepad.Text.Length && removedLength == 0)
                    {
                        // Get the added text from the change
                        string addedText = notepad.Text.Substring(offset, addedLength);

                        if (addedText != "\n")
                        {
                            string msgAdded = "+:length:" + addedLength + ":offset:" + offset + ":text:" + addedText;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                this.SyncManager.AddTextChange(msgAdded);
                            });
                        }
                    }
                    else if (addedLength > 0 && removedLength > 0)
                    {
                        string addedText = notepad.Text.Substring(offset, addedLength);

                        if (!addedText.Contains("\n"))
                        {
                            string update = "=:length:" + removedLength + ":offset:" + offset + ":text:" + addedText;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                this.SyncManager.AddTextChange(update);
                            });
                        }


                        this.deletionFlag = false;
                    }
                    else if (this.deletionFlag == true)
                    {
                        string updateMsg = "-:length:" + addedLength + ":offset:" + offset + ":text:" + removedLength;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.SyncManager.AddTextChange(updateMsg);
                        });

                        this.deletionFlag = false;
                    }
                }

                if (!(this.P2PCM.GetHost()))
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (notepad.CanUndo)
                        {
                            crosshairPlace = this.notepad.CaretIndex;
                            tokens++;
                            notepad.Undo();
                        }
                    }));
                }
            }
            else
            {
                tokens--;
            }
        }

        private void textUpdater()
        {
            while (true)
            {

                string update = "";
                string action = "";
                string strLength = "";
                string strOffset = "";
                string text = "";
                int length = 0;
                int offset = 0;
                int parser = 0;

                update = this.SyncManager.GetLastUpdate();

                if (update != "")
                {
                    for (int i = 0; i < update.Length; i++)
                    {
                        if (update[i] != ':' && parser == ACTION)
                        {
                            action += update[i];
                        }
                        else if (update[i] != ':' && parser == LENGTH)
                        {
                            strLength += update[i];
                        }
                        else if (update[i] != ':' && parser == OFFSET)
                        {
                            strOffset += update[i];
                        }
                        else if (parser == TEXT)
                        {
                            text += update[i];
                        }
                        else if (update[i] == ':' && parser != TEXT)
                        {
                            parser++;
                        }
                    }

                    length = int.Parse(strLength);
                    offset = int.Parse(strOffset);

                    int prevCursosrPosition = this.notepad.CaretIndex;



                    if (action == ADDITION_IDENTIFIER)
                    {

                        this.tokens++;


                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            notepad.Text = notepad.Text.Insert(offset, text);

                            if (offset > prevCursosrPosition)
                            {
                                this.notepad.CaretIndex = prevCursosrPosition;
                            }
                            else if (offset == prevCursosrPosition)
                            {
                                this.notepad.CaretIndex = prevCursosrPosition + 1;
                            }
                            else
                            {
                                this.notepad.CaretIndex = prevCursosrPosition + length;
                            }
                            if (text == "\n")
                            {
                                if (!(this.P2PCM.GetHost()))
                                {
                                    this.notepad.CaretIndex++;
                                }
                            }
                        });

                    }
                    else if (action == DELETION_IDENTIFIER)
                    {
                        int deleteLen = Int32.Parse(text);
                        this.tokens++;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (this.notepad.Text.Length == 1)
                            {
                                this.notepad.Clear();
                                Thread.Sleep(32);
                            }
                            else
                            {
                                // Ensure offset is within bounds before attempting removal
                                if (offset < this.notepad.Text.Length)
                                {
                                    notepad.Text = this.notepad.Text.Remove(offset, deleteLen);
                                }
                                else if (offset == this.notepad.Text.Length && this.notepad.Text.Length != 0)
                                {
                                    notepad.Text = notepad.Text.Remove(offset, deleteLen);
                                }

                                if (offset >= prevCursosrPosition)
                                {
                                    this.notepad.CaretIndex = prevCursosrPosition;
                                }
                                else
                                {
                                    this.notepad.CaretIndex = prevCursosrPosition - deleteLen;
                                }
                            }
                        });
                    }
                    else if (action == MULTIPLE_ACTION_IDENTIFIER)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.tokens += 2;

                            if (this.notepad.Text.Length == 1)
                            {
                                this.notepad.Clear();
                            }
                            else
                            {
                                // Ensure offset is within bounds before attempting gremoval
                                if (offset < this.notepad.Text.Length)
                                {
                                    notepad.Text = this.notepad.Text.Remove(offset, length);
                                }
                                else if (offset == this.notepad.Text.Length && this.notepad.Text.Length != 0)
                                {
                                    notepad.Text = notepad.Text.Remove(offset, length);
                                }

                                if (offset < prevCursosrPosition)
                                {
                                    this.notepad.CaretIndex = prevCursosrPosition - length;
                                }
                            }

                            notepad.Text = notepad.Text.Insert(offset, text);
                            int addedLength = text.Length;

                            if (offset >= prevCursosrPosition)
                            {
                                this.notepad.CaretIndex = prevCursosrPosition;
                            }
                            else
                            {
                                this.notepad.CaretIndex = prevCursosrPosition + addedLength;
                            }
                        });
                    }

                }
            }
        }

        private void notepad_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.sessionStarted == true)
            {
                if (e.Key == Key.Enter)
                {
                    // Prevent the Enter key from adding a newline character
                    e.Handled = true;

                    // Instead, manually add a newline character and move the caret
                    TextBox textBox = (TextBox)sender;
                    int caretIndex = textBox.CaretIndex;

                    textBox.Text = textBox.Text.Insert(caretIndex, "\n");
                    textBox.CaretIndex = caretIndex + 1;

                    // Scroll the TextBox to show the newly added line
                    textBox.ScrollToEnd();

                    // Queue update for other clients
                    string updateAdded = "+:length:" + 0 + ":offset:" + caretIndex + ":text:" + "\n";
                    this.SyncManager.AddTextChange(updateAdded);
                }
                if (e.Key == Key.Back)
                {
                    this.deletionFlag = true;
                }
            }
        }

        private void fileMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}