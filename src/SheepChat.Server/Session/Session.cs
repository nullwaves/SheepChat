using SheepChat.Server.Data.Models;
using SheepChat.Server.Interfaces;
using System;

namespace SheepChat.Server.Sessions
{
    public class Session : ISubSystem
    {
        private ISubSystemHost host;

        public IConnection Connection { get; private set; }

        public string ID
        {
            get { return Connection.ID; }
        }

        public SessionState State { get; set; }

        public User User { get; private set; }

        public delegate void SessionAuthenticatedEventHandler(Session session);

        public event SessionAuthenticatedEventHandler SessionAuthenticated;

        public Session(IConnection conn)
        {
            if(conn != null)
            {
                Connection = conn;
                State = SessionStateManager.Instance.CreateDefaultState(this);
                conn.Send(string.Empty);
            }
        }

        public void AuthenticateSession(User user)
        {
            if (User == null) throw new ArgumentNullException("user", "Authenticated user cannot be null!");
            SessionAuthenticated?.Invoke(this);
        }

        public void ProcessInput(string input) => State.ProcessInput(input);

        public void Write(string data, bool bypassFormatter = false)
        {
            Connection.Send(data, bypassFormatter);
        }

        public void Write()
        {
            Write(string.Empty);
        }

        public void SubscribeToSystem(ISubSystemHost sender)
        {
            host = sender;
        }

        public void UnsubscribeToSystem()
        {
            host = null;
        }

        public void InformSubscribedSystem(string msg)
        {
            host.UpdateSubSystemHost(this, msg);
        }

        public void Start()
        {
            // Nothing to do.
        }

        public void Stop()
        {
            // Nothing to do.
        }
    }
}
