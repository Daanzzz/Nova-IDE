using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;


namespace ServerCommunicatorNS
{
    /// <summary>
    /// Handles communication with the Nova Server using UDP.
    /// </summary>
    public partial class ServerCommunicator
    {
        private UdpClient soc;       // Manages communication
        private IPEndPoint ep;        // Stores server connection details
        private string username;      // User's name for communication
        private string sessionToken;

        private const string SIGNUP_IDENTIFIER = "S"; 
        private const string LOGIN_IDENTIFIER = "L";
        private const string CHECK_CODE_IDENTIFIER = "C";
        private const string CHECK_EMAIL_IDENTIFIER = "V";
        private const string NEW_PASSWORD_IDENTIFIER = "N";
        private const string INVITE_USER = "J";

        /// <summary>
        /// Creates a new ServerCommunicator instance.
        /// </summary>
        public ServerCommunicator()
        {
            
        }


        public void setSessionToken(string token)
        {
            this.sessionToken = token;
        }

        public string getSessionToken()
        {
            return this.sessionToken;
        }
        public string sendSignup(string user, string password, string email)
        {
            string message = $"{SIGNUP_IDENTIFIER}:{user}:{password}:{email}";

            byte[] data = Encoding.UTF8.GetBytes(message);
            this.soc.Send(data, data.Length, this.ep);

            Byte[] receiveBytes = this.soc.Receive(ref this.ep);
            string receivedMessage = Encoding.UTF8.GetString(receiveBytes);
            return receivedMessage;
        }

        public string sendLogin(string user, string password)
        {
            string message = $"{LOGIN_IDENTIFIER}:{user}:{password}";

            byte[] data = Encoding.UTF8.GetBytes(message);
            this.soc.Send(data, data.Length, this.ep);

            Byte[] receiveBytes = this.soc.Receive(ref this.ep);
            string receivedMessage = Encoding.UTF8.GetString(receiveBytes);
            return receivedMessage;
        }

        public string checkEmail(string username, string email)
        {
            string message = $"{CHECK_EMAIL_IDENTIFIER}:{username}:{email}";

            byte[] data = Encoding.UTF8.GetBytes(message);
            this.soc.Send(data, data.Length, this.ep);

            Byte[] receiveBytes = this.soc.Receive(ref this.ep);
            string receivedMessage = Encoding.UTF8.GetString(receiveBytes);
            return receivedMessage;
        }

        public string checkCode(string username, string password, string email, string code)
        {
            string message = $"{CHECK_CODE_IDENTIFIER}:{username}:{password}:{email}:{code}";

            byte[] data = Encoding.UTF8.GetBytes(message);
            this.soc.Send(data, data.Length, this.ep);

            Byte[] receiveBytes = this.soc.Receive(ref this.ep);
            string receivedMessage = Encoding.UTF8.GetString(receiveBytes);
            return receivedMessage;
        }

        public string sendNewPassword(string username ,string password, string email, string code)
        {
            string message = $"{NEW_PASSWORD_IDENTIFIER}:{username}:{password}:{email}:{code}";

            byte[] data = Encoding.UTF8.GetBytes(message);
            this.soc.Send(data, data.Length, this.ep);

            Byte[] receiveBytes = this.soc.Receive(ref this.ep);
            string receivedMessage = Encoding.UTF8.GetString(receiveBytes);
            return receivedMessage;
        }

        public string sendInvite(string username)
        {
            string message = $"{INVITE_USER}:{this.sessionToken}:{username}";

            byte[] data = Encoding.UTF8.GetBytes(message);
            this.soc.Send(data, data.Length, this.ep);

            Byte[] receiveBytes = this.soc.Receive(ref this.ep);
            string receivedMessage = Encoding.UTF8.GetString(receiveBytes);
            return receivedMessage;
        }

        /// <summary>
        /// Connects to a server using the provided IP address and port.
        /// </summary>
        /// <param name="soc">The communication client.</param>
        /// <param name="serverIP">IP address of the server.</param>
        /// <param name="serverPort">Port number of the server.</param>
        public void ConnectToServer(ref UdpClient soc, string serverIP, int serverPort)
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
                this.soc = soc;
                this.ep = ep;
            }
            catch (SocketException e)
            {
                Console.WriteLine("Error connecting to the server: " + e.Message);
            }
        }

        /// <summary>
        /// Collects user and connection details and sends them to the server.
        /// </summary>
        public void GetSessionDetails()
        {
            Console.Write("Enter your username: ");
            string uname = "a"; // For demo; replace with Console.ReadLine() for user input
            this.username = uname;

            Console.Write("Enter the user to connect to: ");
            string otherName = "b"; // For demo; replace with Console.ReadLine() for user input

            string combinedName = $"{uname}:{otherName}";
            Console.WriteLine($"Combined Names: {combinedName}");

            byte[] data = Encoding.UTF8.GetBytes(combinedName);
            this.soc.Send(data, data.Length, this.ep);
        }

        /// <summary>
        /// Receives peer-to-peer details from the server.
        /// </summary>
        /// <returns>The received P2P details.</returns>
        public string GetP2PDetails()
        {
            Byte[] receiveBytes = this.soc.Receive(ref this.ep);
            string receivedMessage = Encoding.ASCII.GetString(receiveBytes);
            return receivedMessage;
        }

        public ref UdpClient getSoc()
        {
            return ref this.soc;
        }
    }
}
