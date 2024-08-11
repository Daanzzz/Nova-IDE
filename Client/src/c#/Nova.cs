using P2PCommunicatorNS;
using ServerCommunicatorNS;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Documents;

namespace Client
{
    /// <summary>
    /// Represents the client-side implementation of the our Nova project.
    /// </summary>
    internal class Nova
    {   
        private const string ServerIP = "127.0.0.1";  // IP address of the server
        private const int ServerPort = 11839;             // Port number for server communication
        private P2PCommunicator p2p;                      // P2PCommunicator instance for peer-to-peer communication
        private ServerCommunicator sc;
        private bool connectNow = false;
        private string p2pDetails;

        /// <summary>
        /// Initializes a new instance of the Nova class.
        /// </summary>
        /// <param name="p2pCM">The P2PCommunicator instance for peer-to-peer communication.</param>
        public Nova(ref P2PCommunicator p2pCM, ref ServerCommunicator sc)
        {
            this.p2p = p2pCM;
            this.sc = sc;
        }

        /// <summary>
        /// Starts the Nova application.
        /// </summary>
        public void Run()
        {
            var flowThread = new Thread(startFlow);
            // detach the thread
            flowThread.IsBackground = true;
            flowThread.Start();
        }

        /// <summary>
        /// Initiates the flow of the Nova application, including server and P2P connection setup.
        /// </summary>
        private void startFlow()
        {
            var soc = new UdpClient();

            // Initializing server thread
            //ServerCommunicator sc = new ServerCommunicator();
            sc.ConnectToServer(ref soc, ServerIP, ServerPort);
            sc.GetSessionDetails();
            //while (!connectNow)
            //{
            //    //Check if the user wants to connect
            //}
            //this.p2p.SetUDPClient(ref soc);
            //this.p2p.ConnectP2P(this.p2pDetails);
            //this.p2p.startSesstion();
        }

        public void setConnect(bool connectNow, string p2pDetails)
        {
            this.connectNow = connectNow;
            this.p2pDetails = p2pDetails;
        }

    }
}
