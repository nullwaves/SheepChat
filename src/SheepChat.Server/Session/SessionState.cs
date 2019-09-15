using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

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
        /// Default constructor for a new SessionState
        /// </summary>
        /// <param name="session">Session entering this state</param>
        public SessionState(Session session)
        {
            Session = session;
        }

        /// <summary>
        /// Process incoming string input from the associated Session.
        /// </summary>
        /// <param name="command">String input from the Session</param>
        public abstract void ProcessInput(string command);

        /// <summary>
        /// Handle anything that needs to be done when a Session leaves a state.
        /// </summary>
        public virtual void OnLeaveState() {}
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
            foreach(var key in metadata.Keys)
            {
                if(key == "statepriority")
                {
                    StatePriority = (int)metadata[key];
                }
            }
        }
    }
}