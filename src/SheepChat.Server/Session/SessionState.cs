using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace SheepChat.Server.Sessions
{
    /// <summary>
    /// Abstract base class for a session state.
    /// </summary>
    public abstract class SessionState
    {
        /// <summary>
        /// Session that is currently in this state.
        /// </summary>
        protected Session Session { get; set; }

        /// <summary>
        /// Gets the textual representation of the data still waiting to be returned as an input string.
        /// </summary>
        public StringBuilder Buffer { get; set; }

        /// <summary>
        /// Default constructor for a new SessionState
        /// </summary>
        /// <param name="session">Session entering this state</param>
        public SessionState(Session session)
        {
            Session = session;
            Buffer = new StringBuilder();
        }

        /// <summary>
        /// Process incoming string input from the associated Session.
        /// </summary>
        /// <param name="command">String input from the Session</param>
        public abstract void ProcessInput(string command);

        /// <summary>
        /// Process raw byte data from the connection.
        /// </summary>
        /// <param name="data">Byte data sent from client.</param>
        public virtual void ProcessData(byte[] data)
        {
            // TODO: Catch and handle escape sequences
            foreach (byte b in data)
            {
                switch (b)
                {
                    case 13:
                        ProcessInput(Buffer.ToString());
                        Buffer.Clear();
                        break;

                    case byte n when (n > 31 && n < 127):
                        Buffer.Append(((char)b));
                        break;

                    case 8:
                        if (Buffer.Length > 0)
                        {
                            Buffer.Remove(Buffer.Length - 1, 1);
                        }
                        break;

                    case 127:
                        break;
                }
            }
        }

        /// <summary>
        /// Handle anything that needs to be done when a Session leaves a state.
        /// </summary>
        public virtual void OnLeaveState()
        { }
    }

    /// <summary>
    /// SessionState export attribute for tagging SessionStates to be imported on system boot.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportSessionStateAttribute : ExportAttribute
    {
        /// <summary>
        /// State priority determines the default state a session is put into after connecting.
        /// </summary>
        public int StatePriority { get; set; }

        /// <summary>
        /// Default constructor for a exported SessionState. Sets the state priority.
        /// </summary>
        /// <param name="statePriority">Priority of the SessionState when determining the default session state on connection</param>
        public ExportSessionStateAttribute(int statePriority)
            : base(typeof(SessionState))
        {
            this.StatePriority = statePriority;
        }

        /// <summary>
        /// Constructor loading metadata from loosely imported session states.
        /// </summary>
        /// <param name="metadata">SessionState metadata</param>
        public ExportSessionStateAttribute(IDictionary<string, object> metadata)
        {
            foreach (var key in metadata.Keys)
            {
                if (key == "statepriority")
                {
                    StatePriority = (int)metadata[key];
                }
            }
        }
    }
}