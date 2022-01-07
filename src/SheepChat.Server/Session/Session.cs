using SheepChat.Server.Data.Models;
using SheepChat.Server.Interfaces;
using System;

namespace SheepChat.Server.Sessions
{
    /// <summary>
    /// Session sub-system, handled by a <see cref="SessionManager"/>
    /// </summary>
    public class Session : ISubSystem
    {
        private ISubSystemHost host;

        /// <summary>
        /// Socket connection associated with the Session
        /// </summary>
        public IConnection Connection { get; private set; }

        /// <summary>
        /// Session ID string
        /// </summary>
        public string ID
        {
            get { return Connection.ID; }
        }

        private SessionState _state;

        /// <summary>
        /// Session's current state
        /// </summary>
        public SessionState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state?.OnLeaveState();
                _state = value;
            }
        }

        /// <summary>
        /// An authenticated users associated with the session.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// Event handler delegate for Session related events
        /// </summary>
        /// <param name="session">Session that caused the event</param>
        public delegate void SessionEventHandler(Session session);

        /// <summary>
        /// Event to be/triggered when a Session has been authenticated.
        /// </summary>
        public event SessionEventHandler SessionAuthenticated;

        /// <summary>
        /// Session constructor for incoming connections.
        /// </summary>
        /// <param name="conn">Incoming connection</param>
        public Session(IConnection conn)
        {
            if (conn != null)
            {
                Connection = conn;
                User = null;
                State = SessionStateManager.Instance.CreateDefaultState(this);
                Connection.Send(string.Empty);
            }
        }

        /// <summary>
        /// Mark session as authenticated with a non-null user
        /// </summary>
        /// <param name="user">User that was authenticated in this session</param>
        public void AuthenticateSession(User user)
        {
            User = user ?? throw new ArgumentNullException("user", "Authenticated user cannot be null!");
            SessionAuthenticated?.Invoke(this);
        }

        /// <summary>
        /// Process incoming data from the connection
        /// </summary>
        /// <param name="input">Input that is either a command or a message</param>
        public void ProcessInput(string input)
        {
            if (User != null && input.StartsWith(CommandManager.Instance.Trigger))
            {
                CommandManager.Instance.ProcessCommand(this, input.Substring(1));
            }
            else
            {
                State.ProcessInput(input);
            }
        }

        /// <summary>
        /// Send a string of data to the connection.
        /// </summary>
        /// <param name="data">String of data to be sent</param>
        /// <param name="bypassFormatter">Flag for pre-formatted data or data that should not be formatted otherwise</param>
        public void Write(string data, bool bypassFormatter = false)
        {
            Connection.Send(data, bypassFormatter);
        }

        /// <summary>
        /// Keep that connection awake.
        /// </summary>
        public void Write()
        {
            Write(string.Empty);
        }

        /// <summary>
        /// Subscribe to sub-system host
        /// </summary>
        /// <param name="sender">Sub-System host to subscribe to</param>
        public void SubscribeToSystem(ISubSystemHost sender)
        {
            host = sender;
        }

        /// <summary>
        /// Unsubscribe from current Sub-System host.
        /// </summary>
        public void UnsubscribeToSystem()
        {
            host = null;
        }

        /// <summary>
        /// Pass message up to Sub-System host.
        /// </summary>
        /// <param name="msg"></param>
        public void InformSubscribedSystem(string msg)
        {
            host.UpdateSubSystemHost(this, msg);
        }

        /// <summary>
        /// Start the session. (Does nothing.)
        /// </summary>
        public void Start()
        {
            // Nothing to do.
        }

        /// <summary>
        /// Stop the session. (Does nothing.)
        /// </summary>
        public void Stop()
        {
            // Nothing to do.
        }
    }
}