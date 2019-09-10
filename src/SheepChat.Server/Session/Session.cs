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

        private SessionState _state;

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

        public User User { get; private set; }

        public delegate void SessionEventHandler(Session session);

        public event SessionEventHandler SessionAuthenticated;

        public Session(IConnection conn)
        {
            if(conn != null)
            {
                Connection = conn;
                User = null;
                State = SessionStateManager.Instance.CreateDefaultState(this);
                Connection.Send(string.Empty);
            }
        }

        public void AuthenticateSession(User user)
        {
            User = user ?? throw new ArgumentNullException("user", "Authenticated user cannot be null!");
            SessionAuthenticated?.Invoke(this);
        }

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
