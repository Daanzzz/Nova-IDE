using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System;
using HostNS;
using SynchronizationManagement;
using SessionMemberNS;
using System.Reflection.Metadata.Ecma335;
using System.IO;

namespace P2PCommunicatorNS
{
    /// <summary>
    /// Manages peer-to-peer communication between hosts and members in a collaborative session.
    /// </summary>
    public partial class P2PCommunicator
    {
        // Constants
        public const int SEND_DELAY = 10; // Sleep time in milliseconds for update sending
        public const string HOST_IDENTIFIER = "H";
        public const char INFO_IDENTIFIER = 'I';
        public const char FILES_IDENTIFIER = 'F';

        // Parsing constants
        private const int IP = 0;
        private const int PORT = 1;
        private const int TYPE = 2;

        // Greeting messages
        private const int GREETING_MESSAGES_AMOUNT = 10;

        // Communication components
        public UdpClient P2PSoc;
        private IPEndPoint ep;
        private List<IPEndPoint> epList;

        // Synchronization and session settings
        private SynchronizationManager SyncManager;
        private SessionHost Host;
        private SessionMember Member;
        private bool hostFlag;

        // Connection details
        private string ip;
        private int port;

        private Dictionary<string, string> files;
        private bool filesRecieved;

        /// <summary>
        /// Initializes a new instance of the <see cref="P2PCommunicator"/> class.
        /// </summary>
        /// <param name="SyncManager">The synchronization manager for managing text updates.</param>
        public P2PCommunicator(ref SynchronizationManager SyncManager)
        {
            this.epList = new List<IPEndPoint>();
            this.SyncManager = SyncManager;
            this.Host = new SessionHost(this, ref this.SyncManager);
            this.Member = new SessionMember(this, ref this.SyncManager);
            this.hostFlag = true;
            this.files = new Dictionary<string, string>();
            this.filesRecieved = false;
        }

        /// <summary>
        /// Sets the UDP client for peer-to-peer communication.
        /// </summary>
        /// <param name="communicationSoc">The UDP client for communication.</param>
        public void SetUDPClient(ref UdpClient communicationSoc)
        {
            this.P2PSoc = communicationSoc;
        }



        /// <summary>
        /// Connects to a peer for peer-to-peer communication using the provided details.
        /// </summary>
        /// <param name="details">The connection details in the format "IP:PORT:TYPE".</param>
        public void ConnectP2P(string details)
        {
            string ip = "";
            string tempPort = "";
            int port = 0;
            string type = "";
            int parser = 0;

            // Parse connection details
            for (int i = 0; i < details.Length; i++)
            {
                if (details[i] != ':' && parser == IP)
                {
                    ip += details[i];
                }
                else if (details[i] != ':' && parser == PORT)
                {
                    tempPort += details[i];
                }
                else if (details[i] != ':' && parser == TYPE)
                {
                    type += details[i];
                }
                else if (details[i] == ':')
                {
                    parser++;
                }
            }

            port = int.Parse(tempPort);
            this.ip = ip;
            this.port = port;

            // Add peer as host or member based on the type
            if (type == HOST_IDENTIFIER)
            {
                this.Host.AddIPEP(ip, port);
            }
            else
            {
                this.hostFlag = false;
                this.Member.UpdateHostIPEP(ip, port);
            }
        }

        /// <summary>
        /// Listens for incoming messages and processes them accordingly.
        /// </summary>
        public void P2PListener()
        {
            while (true)
            {
                IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0); // Initialize an IPEndPoint for the sender
                Byte[] receiveBytes = this.P2PSoc.Receive(ref senderEndPoint);
                string receivedMessage = Encoding.ASCII.GetString(receiveBytes);

                if(receivedMessage.Length > 0)
                {
                    //File.WriteAllText("E:\\NovaProject\\log.txt", receivedMessage + "\n\n");
                    // Check if it's an update message
                    if (receivedMessage[0] == '+' || receivedMessage[0] == '-' || receivedMessage[0] == '=') // checking if it's a update message
                    {
                        // Enqueue the update for further processing based on the host flag
                        if (this.hostFlag)
                        {
                            this.Host.updateDeliveryQueue.Enqueue(new Tuple<string, IPEndPoint, bool>(receivedMessage, senderEndPoint, true)); // host got update from any client
                        }
                        else
                        {
                            this.Member.UpdateIDEQueue.Enqueue(receivedMessage);
                        }
                    }
                    else if (receivedMessage[0] == INFO_IDENTIFIER)
                    {
                        this.Member.ConfigureSession(receivedMessage);
                    }
                    else if (receivedMessage[0] == FILES_IDENTIFIER)
                    {
                        this.files = this.Member.ReceiveAndParseMessage(receivedMessage);
                        this.filesRecieved = true;
                    }
                }
                
            }
        }


        public Dictionary<string, string> getFiles()
        {
            while(!this.filesRecieved)
            {
                //need to wait
            }
            return this.files;
        }

        public void sendFiles(Dictionary<string, string> filesToSend)
        {
            this.Host.SendFiles(filesToSend);
        }

        /// <summary>
        /// Starts the peer-to-peer session by updating endpoints and initiating listener threads.
        /// </summary>
        public void startSesstion()
        {
            // Update endpoint based on host flag
            if (this.hostFlag == false)
            {
                this.ep = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
            }
            else
            {
                this.epList.Add(new IPEndPoint(IPAddress.Parse(this.ip), this.port));
            }

            // Start the listener thread
            var listenerThread = new Thread(P2PListener);
            listenerThread.IsBackground = true;
            listenerThread.Start();

            string msg;

            // Send greeting messages
            for (int i = 0; i < GREETING_MESSAGES_AMOUNT; i++)
            {
                msg = "b";
                    
                byte[] data = Encoding.UTF8.GetBytes(msg);
                if(hostFlag != true)
                {
                    this.P2PSoc.Send(data, data.Length, this.ep); // Send data to this.other using the new UdpClient
                    //Thread.Sleep(SEND_DELAY);
                }
                else
                {
                    this.P2PSoc.Send(data, data.Length, this.epList.Last());
                    //Thread.Sleep(SEND_DELAY);
                }

            }

            // Start the session as a host or member based on the host flag
            if (this.hostFlag)
            {
                this.Host.StartSessionAsHost();
                this.Host.DelieverMemberInfo(this.epList.Last());
            }
            else
            {
                this.Member.StartSessionAsMember();
            }
        }

        public bool GetHost()
        {
            return this.hostFlag;
        }

        public char GetID()
        {
            if (GetHost())
            {
                return '0'; // '0' = Host identifier
            }
            else
            {
                return this.Member.GetMemberID();
            }
        }



    }
}
