using System.Collections.Generic;

namespace SynchronizationManagement
{
    /// <summary>
    /// Manages the synchronization of updates between the IDE textbox and other classes.
    /// </summary>
    public partial class SynchronizationManager
    {
        // Queues to store updates
        private Queue<string> TextUpdateQueue;   // Updates queued to the IDE itself
        private Queue<string> TextChangesQueue;  // Updates that came from the IDE itself

        /// <summary>
        /// Initializes a new instance of the SynchronizationManager class.
        /// </summary>
        public SynchronizationManager()
        {
            this.TextUpdateQueue = new Queue<string>();
            this.TextChangesQueue = new Queue<string>();
        }

        /// <summary>
        /// Adds an update from the IDE to the queue for updates queued to the IDE itself.
        /// </summary>
        /// <param name="update">The update to be added to the queue.</param>
        public void AddIDEUpdate(string update)
        {
            if(update != null)
            {
                this.TextUpdateQueue.Enqueue(update);
            }
            
        }

        /// <summary>
        /// Adds a text change from the IDE to the queue for updates that came from the IDE itself.
        /// </summary>
        /// <param name="update">The text change to be added to the queue.</param>
        public void AddTextChange(string update)
        {
            this.TextChangesQueue.Enqueue(update);
        }

        /// <summary>
        /// Retrieves and dequeues the last update from the queue for updates queued to the IDE itself.
        /// </summary>
        /// <returns>The last update from the queue.</returns>
        public string GetLastUpdate()
        {
            if (TextUpdateQueue.Count > 0)
            {
                return TextUpdateQueue.Dequeue();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Retrieves and dequeues the last text change from the queue for updates that came from the IDE itself.
        /// </summary>
        /// <returns>The last text change from the queue.</returns>
        public string GetLastChange()
        {
            if (TextChangesQueue.Count > 0)
            {
                return TextChangesQueue.Dequeue();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Gets a reference to the queue for updates queued to the IDE itself.
        /// </summary>
        /// <returns>A reference to the TextUpdateQueue.</returns>
        public ref Queue<string> GetTextUpdateQueue()
        {
            return ref this.TextUpdateQueue;
        }

        /// <summary>
        /// Gets a reference to the queue for updates that came from the IDE itself.
        /// </summary>
        /// <returns>A reference to the TextChangesQueue.</returns>
        public ref Queue<string> GetTextChnagesQueue()
        {
            return ref this.TextChangesQueue;
        }
    }
}
