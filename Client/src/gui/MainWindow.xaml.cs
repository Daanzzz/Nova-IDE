using System.Data.Common;
using System.Windows;
using System.Windows.Input;
using Client.GUI_Files;
using Client.src.gui;
using P2PCommunicatorNS;
using SynchronizationManagement;
using ServerCommunicatorNS;

namespace Client
{
    public partial class MainWindow : Window
    {
        private MenuPage menuPage;
        private LanguagePage languagePage;
        private UnderWork underWorkPage;
        private ProjectDetails projectDetails;
        private Ide idePage;
        private Nova program;
        private FirstPage first;
        private Login login;
        private Signup signup;
        private EmailVerfication verification;
        private PasswordRecovery recovery;
        private NewPassword newPassword;
        private IDEtest test;
        private StartSession startSession;
        private NewFile newFile;
        private DeleteFile deleteFile;

        private P2PCommunicator p2pCM;
        private SynchronizationManager SyncMngr;


        public MainWindow()
        {
            InitializeComponent();

            SynchronizationManager SyncMngr = new SynchronizationManager();
            P2PCommunicator p2pCM = new P2PCommunicator(ref SyncMngr);
            ServerCommunicator sc = new ServerCommunicator();

            this.p2pCM = p2pCM;
            this.SyncMngr = SyncMngr;

            this.program = new Nova(ref p2pCM, ref sc);
            program.Run();

            this.menuPage = new MenuPage();
            this.languagePage = new LanguagePage();
            this.underWorkPage = new UnderWork();
            this.projectDetails = new ProjectDetails();
            this.idePage = new Ide(ref this.p2pCM, ref this.SyncMngr);
            this.first = new FirstPage();
            this.login = new Login(ref sc);
            this.signup = new Signup(ref sc);
            this.verification = new EmailVerfication(ref sc);
            this.recovery = new PasswordRecovery(ref sc);
            this.newPassword = new NewPassword(ref sc);
            this.test = new IDEtest();
            this.startSession = new StartSession(ref sc, ref p2pCM);
            this.newFile = new NewFile();
            this.deleteFile = new DeleteFile();

            MainContent.Content = first;
            //MainContent.Content = test;

            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = Width;
            double windowHeight = Height;

            Left = (screenWidth - windowWidth) / 2;
            Top = (screenHeight - windowHeight) / 2 - 100;
        }

        public void removeFile(string fileName)
        {
            this.idePage.removeFile(fileName);
        }
        public void updateNewFileName(string fileName)
        {
            this.idePage.setNewFileName(fileName);
        }
        public void updateProjectDetails(string projectPath, string filesPath, string projectName)
        {
            this.idePage.setProjectDetails(projectPath, filesPath, projectName);
        }
        public void connectP2P(string p2pDetails)
        {
            this.program.setConnect(true, p2pDetails);
        }
        public void updateEmailInPassword(string email)
        {
            this.newPassword.setEmail(email);
        }
        public void updateCode(string code)
        {
            this.newPassword.setCode(code);
        }
        public void updateUsername(string username)
        {
            this.newPassword.setUsername(username);
        }

        public void updateUsernameVer(string username)
        {
            this.verification.setUsername(username);
        }
        public void updatePasswordVer(string password)
        {
            this.verification.setPassword(password);
        }
        public void updateAction(string action)
        {
            this.verification.setAction(action);
        }
        public void updateEmail(string email)
        {
            this.verification.setEmail(email);
        }
        public void updateConnectedUser(string username)
        {
            this.idePage.setOtherUser(username);
        }

        public void openDeleteFilePage()
        {
            MainContent.Content = deleteFile;
        }
        public void openNewFilePage()
        {
            MainContent.Content = newFile;
        }
        public void openStartSessionPage()
        {
            MainContent.Content = startSession;
        }
        public void openNewPasswordPage()
        {
            MainContent.Content = newPassword;
        }
        public void OpenRecoveryPage()
        {
            MainContent.Content = recovery;
        }
        public void OpenVerificationPage()
        {
            MainContent.Content = verification;
        }
        public void OpenFirstPage()
        {
            MainContent.Content = first;
        }
        public void OpenLoginPage()
        {
            MainContent.Content = login;
        }

        public void OpenSignupPage()
        {
            MainContent.Content = signup;
        }
        public void OpenFileCreationPage()
        {
            MainContent.Content = languagePage;
        }

        public void openUnderWorkPage()
        {
            MainContent.Content = underWorkPage;
        }

        public void openNewProjectPage()
        {
            MainContent.Content = projectDetails;
        }

        public void OpenMenuPage()
        {
            MainContent.Content = menuPage;
        }

        public void OpenIdePage(bool isConnected, string username)
        {
            if (isConnected)
            {
                this.idePage.setOtherUser(username);
            }

            MainContent.Content = idePage;


        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CustomTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

    }
}
