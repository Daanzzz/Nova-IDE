using System.Collections.Generic;
using System;
using System.Net;
using System.Text;
using System.Threading;
using P2PCommunicatorNS;
using SynchronizationManagement;
using System.Linq;

namespace HostNS
{
    /// <summary>
    /// Manages communication between the IDE textbox and networked session members as a host in the session.
    /// </summary>
    public partial class SessionHost
    {
        // Constants
        public const int SEND_DELAY = 20; // Sleep time in milliseconds for update sending
        public const int MESSAGE_ID_GAP = 50;
        public const char INFO_IDENTIFIER = 'I';
        public const char FILES_IDENTIFIER = 'F';

        // Networks related
        private P2PCommunicator P2PCM;   // Handles peer-to-peer communication
        private List<IPEndPoint> epList; // List of endpoints for connected session members to host
        private Dictionary<int, int> UpdateIDs;
        private int lastUserID;
        private int lastFirstMessageID;

        // Update queues
        public Queue<Tuple<string, IPEndPoint, bool>> updateDeliveryQueue;

        private SynchronizationManager SyncManager;  // Manages synchronization of updates between IDE and networked session members

        /// <summary>
        /// Initializes a new instance of the SessionHost class.
        /// </summary>
        /// <param name="P2PCM">The P2PCommunicator for network communication.</param>
        /// <param name="SyncMngr">The SynchronizationManager for managing updates.</param>
        public SessionHost(P2PCommunicator P2PCM, ref SynchronizationManager SyncMngr)
        {
            this.P2PCM = P2PCM;
            this.updateDeliveryQueue = new Queue<Tuple<string, IPEndPoint, bool>>();
            this.SyncManager = SyncMngr;
            this.epList = new List<IPEndPoint>();
            this.UpdateIDs = new Dictionary<int, int>();
            this.lastUserID = 1;
            this.lastFirstMessageID = 0;
        }

        /// <summary>
        /// Initiates the session as a host, starting update distribution and IDE update scanning threads.
        /// </summary>
        public void StartSessionAsHost()
        {

            var upddateDistribution = new Thread(UpdateDistributor);
            upddateDistribution.IsBackground = true;
            upddateDistribution.Start();

            var updateScan = new Thread(IDEUpdateScanner);
            updateScan.IsBackground = true;
            updateScan.Start();
        }

        public void SendFiles(Dictionary<string, string> files)
        {
            string message = string.Empty;
            message += FILES_IDENTIFIER.ToString();
            foreach (var file in files)
            {
                message += "|||";
                message += file.Key;
                message += "|||";
                if(file.Value == string.Empty)
                {
                    message += "0";
                }
                else
                {
                    message += file.Value;
                }
            }

            byte[] data = Encoding.UTF8.GetBytes(message);
            this.P2PCM.P2PSoc.Send(data, data.Length, epList.First());
        }
        public void DelieverMemberInfo(IPEndPoint ep)
        {
            // H:USERID:MAXMSGID
            string infoMessage = INFO_IDENTIFIER.ToString() + lastUserID.ToString() + (lastFirstMessageID + MESSAGE_ID_GAP).ToString();

            byte[] data = Encoding.UTF8.GetBytes(infoMessage);
            this.P2PCM.P2PSoc.Send(data, data.Length, ep);

            this.lastUserID++;
            this.lastFirstMessageID += MESSAGE_ID_GAP;
        }

        /// <summary>
        /// Continuously updates session members with pending updates from the update queue.
        /// </summary>
        /// <remarks>
        /// This method is responsible for dequeuing updates from the update queue and delivering
        /// them to the specified list of endpoints, excluding the source endpoint. If the update
        /// is marked for IDE update, it is enqueued for further processing.
        /// </remarks>
        public void UpdateDistributor()
        {
            while (true)
            {
                // Check for pending updates
                if (this.updateDeliveryQueue.Count != 0)
                {
                    // Dequeue the next update
                    Tuple<string, IPEndPoint, bool> updateTuple = this.updateDeliveryQueue.Dequeue();
                    string VerifiedUpdate = ValidateUpdate(updateTuple.Item1);

                    // Broadcast the update to all endpoints except the source
                    BroadcastUpdate(updateTuple);

                    // Enqueue the update for IDE processing if necessary
                    EnqueueForIDEUpdate(updateTuple);
                }
            }
        }

        public string ValidateUpdate(string update)
        {
            string newUpdate = "";

            

            return newUpdate;
        }

        /// <summary>
        /// Scans for IDE updates and broadcasts them to all specified endpoints.
        /// </summary>
        public void IDEUpdateScanner()
        {
            while (true)
            {
                if (this.SyncManager.GetTextChnagesQueue().Count != 0)
                {
                    Tuple<string, IPEndPoint, bool> updateTuple = new Tuple<string, IPEndPoint, bool>(this.SyncManager.GetLastChange(), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0), false);

                    BroadcastUpdate(updateTuple);
                }
            }
        }

        /// <summary>
        /// Broadcasts an update message to all specified endpoints except the source.
        /// </summary>
        /// <param name="updateTuple">The update information tuple.</param>
        private void BroadcastUpdate(Tuple<string, IPEndPoint, bool> updateTuple)
        {
            foreach (IPEndPoint endpoint in this.epList)
            {
                //if (!endpoint.Equals(updateTuple.Item2))
                //{
                //Thread.Sleep(SEND_DELAY);
                byte[] data = Encoding.UTF8.GetBytes(updateTuple.Item1);
                this.P2PCM.P2PSoc.Send(data, data.Length, endpoint);
          
                //}
            }
        }

        /// <summary>
        /// Enqueues an update message for further processing in the IDE updater queue.
        /// </summary>
        /// <param name="updateTuple">The update information tuple.</param>
        private void EnqueueForIDEUpdate(Tuple<string, IPEndPoint, bool> updateTuple)
        {
            if (updateTuple.Item3)
            {
                this.SyncManager.AddIDEUpdate(updateTuple.Item1);
            }
        }

        /// <summary>
        /// Adds a new endpoint to the list of hosts.
        /// </summary>
        /// <param name="ip">The IP address of the host.</param>
        /// <param name="port">The port number of the host.</param>
        public void AddIPEP(string ip, int port)
        {
            IPAddress HostIP = IPAddress.Parse(ip);
            this.epList.Add(new IPEndPoint(HostIP, port));
        }
     }
}
