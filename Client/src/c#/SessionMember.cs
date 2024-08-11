using P2PCommunicatorNS;
using SynchronizationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace SessionMemberNS
{
    /// <summary>
    /// Represents a session member that communicates with the host and manages updates.
    /// </summary>
    public partial class SessionMember
    {
        // Constants
        public const int SEND_DELAY = 20; // Sleep time in milliseconds for update sending
        public const char FILES_IDENTIFIER = 'F';

        // Networks related
        private P2PCommunicator P2PCM;  // Handles peer-to-peer communication
        private IPEndPoint HostIPEP;    // Represents the endpoint of the session host
        private char memberID;
        private int maxMessageID;

        private SynchronizationManager SyncManager;  // Manages synchronization of updates between IDE and networked hosts

        // Update queues
        public Queue<string> UpdateIDEQueue;  // Queue for updates intended for the IDE


        /// <summary>
        /// Initializes a new instance of the SessionMember class.
        /// </summary>
        /// <param name="P2PCM">The P2PCommunicator for network communication.</param>
        /// <param name="SyncMngr">The SynchronizationManager for managing updates.</param>
        public SessionMember(P2PCommunicator P2PCM, ref SynchronizationManager SyncMngr)
        {
            this.P2PCM = P2PCM;
            this.SyncManager = SyncMngr;
            this.HostIPEP = null;
            this.UpdateIDEQueue = new Queue<string>();
            this.memberID = '9';
            this.maxMessageID = 0;
        }

        /// <summary>
        /// Initiates the session as a member, starting threads for update processing and IDE update scanning.
        /// </summary>
        public void StartSessionAsMember()
        {
            var updateGetter = new Thread(EnqueueForIDEUpdate);
            updateGetter.IsBackground = true;
            updateGetter.Start();

            var updateScan = new Thread(IDEUpdateScanner);
            updateScan.IsBackground = true;
            updateScan.Start();
        }

        public void ConfigureSession(string ConfigurationMessage)
        {
            this.memberID = ConfigurationMessage[1];
            this.maxMessageID = AnalyzeConfigurationMessagage(ConfigurationMessage);
        }

        public int AnalyzeConfigurationMessagage(string ConfigurationMessage)
        {
            const int maxMsgIDStartIndex = 2;
            string result = "";

            for (int i = 2; i < ConfigurationMessage.Length; i++)
            {
                result += ConfigurationMessage[i];
            }

            return int.Parse(result);
        }

        /// <summary>
        /// Monitors the UpdateIDEQueue and forwards updates to the SynchronizationManager.
        /// </summary>
        private void EnqueueForIDEUpdate()
        {
            while (true)
            {
                if (this.UpdateIDEQueue.Count != 0)
                {
                    this.SyncManager.AddIDEUpdate(this.UpdateIDEQueue.Dequeue());
                }
            }
        }

        /// <summary>
        /// Scans for IDE updates and sends them to the session host.
        /// </summary>
        public void IDEUpdateScanner()
        {
            while (true)
            {
                if (this.SyncManager.GetTextChnagesQueue().Count != 0)
                {
                    //Thread.Sleep(SEND_DELAY);
                    byte[] data = Encoding.UTF8.GetBytes(this.SyncManager.GetLastChange());
                    this.P2PCM.P2PSoc.Send(data, data.Length, this.HostIPEP);
                }
            }
        }

        /// <summary>
        /// Updates the endpoint information of the session host.
        /// </summary>
        /// <param name="ip">The IP address of the host.</param>
        /// <param name="port">The port number of the host.</param>
        public void UpdateHostIPEP(string ip, int port)
        {
            // Updating host endpoint
            IPAddress HostIP = IPAddress.Parse(ip);
            this.HostIPEP = new IPEndPoint(HostIP, port);
        }

        public char GetMemberID()
        {
            return this.memberID;
        }

        public Dictionary<string, string> ReceiveAndParseMessage(string receivedMessage)
        {

            string[] fileEntries = receivedMessage.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

            // Dictionary to store received files
            Dictionary<string, string> receivedFiles = new Dictionary<string, string>();
            int filesCounter = 0;
            string fileName = string.Empty;
            string fileContent = string.Empty;

            // strting from index 1 beacuase 0 is the identifier
            for (int i = 1; i < fileEntries.Length; i++)
            {
                if(filesCounter == 0)
                {
                    fileName = fileEntries[i];
                }
                else if(filesCounter == 1)
                {
                    fileContent = fileEntries[i];
                }


                filesCounter++;
                if(filesCounter == 2)
                {
                    receivedFiles.Add(fileName, fileContent);
                    filesCounter = 0;
                }
            }

            return receivedFiles;
        }
    }
}
