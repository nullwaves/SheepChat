using System.Collections.Generic;
using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;

namespace SheepChat.Server
{
    public class SessionManager : Manager
    {
        private static readonly SessionManager Singleton = new SessionManager();

        public static SessionManager Instance
        {
            get
            {
                return Singleton;
            }
        }

        public Dictionary<string, Session> Sessions { get; private set; }

        public SessionManager()
        {
            Sessions = new Dictionary<string, Session>();
        }

        public void OnSessionConnect(IConnection conn)
        {
            CreateSession(conn);
        }

        public void OnSessionDisconnect(IConnection conn)
        {
            lock(Sessions)
            {
                RemoveSession(conn.ID);
            }
        }

        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");

            // Do anything extra here.

            SystemHost.UpdateSystemHost(this, "Started");
        }

        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");

            lock (Sessions)
            {
                Sessions.Clear();
            }

            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        private Session CreateSession(IConnection connection)
        {
            connection.Send("Status: Connected");

            var sess = new Session(connection);
            sess.SessionAuthenticated += OnSessionAuthenticated;
            sess.SubscribeToSystem(this);

            lock (Sessions)
            {
                Sessions.Add(connection.ID, sess);
            }

            return sess;
        }

        private void OnSessionAuthenticated(Session session)
        {
            SystemHost.UpdateSystemHost(this, session.ID + " - Authenticated");
        }

        private void RemoveSession(string sessionId)
        {
            lock(Sessions)
            {
                if(Sessions.ContainsKey(sessionId))
                {
                    var sess = Sessions[sessionId];
                    sess.UnsubscribeToSystem();
                    Sessions.Remove(sessionId);
                }
            }
        }
    }
}