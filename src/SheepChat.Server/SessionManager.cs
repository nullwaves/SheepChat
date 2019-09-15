using System;
using System.Collections.Generic;
using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;

namespace SheepChat.Server
{
    /// <summary>
    /// Manager system for Sessions
    /// </summary>
    public class SessionManager : Manager
    {
        /// <summary>
        /// Manager system name.
        /// </summary>
        public override string Name { get { return "Session"; } }

        /// <summary>
        /// Singleton instance of the manager system to prevent duplicate systems.
        /// </summary>
        public static SessionManager Instance { get; } = new SessionManager();

        /// <summary>
        /// Dictionary of connected sessions.
        /// </summary>
        public Dictionary<string, Session> Sessions { get; private set; }

        /// <summary>
        /// Default constructor for the Session manager system.
        /// </summary>
        private SessionManager()
        {
            Sessions = new Dictionary<string, Session>();
        }

        /// <summary>
        /// Initiate a session for a freshly connected connection.
        /// </summary>
        /// <param name="conn">Connection that has connected</param>
        public void OnSessionConnect(IConnection conn)
        {
            CreateSession(conn);
        }

        /// <summary>
        /// Handle the destruction and disposal of a session from which's connection has been disconnected.
        /// </summary>
        /// <param name="conn">Connection that disconnected</param>
        public void OnSessionDisconnect(IConnection conn)
        {
            lock(Sessions)
            {
                RemoveSession(conn.ID);
            }
        }

        /// <summary>
        /// Passes santized input to the session to handle.
        /// </summary>
        /// <param name="conn">Connection that sent the input</param>
        /// <param name="input">Input sent from the connection</param>
        public void OnInputReceived(IConnection conn, string input)
        {
            Session session = null;
            lock(Sessions)
            {
                session = Sessions.ContainsKey(conn.ID) ? Sessions[conn.ID] : null;
            }
            session?.ProcessInput(input);
        }

        /// <summary>
        /// Start the Session manager system.
        /// </summary>
        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");

            // Do anything extra here.

            SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>
        /// Stop the Session manager system.
        /// </summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");

            lock (Sessions)
            {
                Sessions.Clear();
            }

            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>
        /// Create a new session from an incoming connection.
        /// </summary>
        /// <param name="connection">Connection that just connected</param>
        /// <returns>A new session linked to the new connection</returns>
        private Session CreateSession(IConnection connection)
        {
            connection.Send("Status: Connected" + Environment.NewLine);

            var sess = new Session(connection);
            sess.SessionAuthenticated += OnSessionAuthenticated;
            sess.SubscribeToSystem(this);

            lock (Sessions)
            {
                Sessions.Add(connection.ID, sess);
            }

            return sess;
        }

        /// <summary>
        /// Event handler for a successfully authenticated session.
        /// </summary>
        /// <param name="session">Session that was authenticated</param>
        private void OnSessionAuthenticated(Session session)
        {
            SystemHost.UpdateSystemHost(this, session.ID + " - Authenticated");
        }

        /// <summary>
        /// Remove a session that is no longer connected.
        /// </summary>
        /// <param name="sessionId">ID of the session to remove</param>
        private void RemoveSession(string sessionId)
        {
            lock(Sessions)
            {
                if(Sessions.ContainsKey(sessionId))
                {
                    var sess = Sessions[sessionId];
                    sess.State?.OnLeaveState();
                    sess.UnsubscribeToSystem();
                    Sessions.Remove(sessionId);
                }
            }
        }
    }

    /// <summary>
    /// Instance exporter for the <see cref="SessionManager"/> system.
    /// </summary>
    [ExportInstance]
    public class SessionManagerInstance : InstanceExporter
    {
        /// <summary>
        /// Singleton instance of the <see cref="SessionManager"/>.
        /// </summary>
        public override ISystem Instance => SessionManager.Instance;

        /// <summary>
        /// Instance type of <see cref="SessionManager"/>.
        /// </summary>
        public override Type InstanceType => typeof(SessionManager);
    }
}