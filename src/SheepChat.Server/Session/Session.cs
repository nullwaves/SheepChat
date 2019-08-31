using SheepChat.Server.Interfaces;

namespace SheepChat.Server.Session
{
    public class Session : ISubSystem
    {
        private ISubSystemHost host;

        public IConnection Connection { get; private set; }

        public SessionState State { get; set; }

        // public User User { get; set; }

        public Session(IConnection conn)
        {
            if(conn != null)
            {
                Connection = conn;
                State = SessionStateManager.Instance.CreateDefaultState(this);
                conn.Send(string.Empty);
            }
        }

        public void SubscribeToSystem(ISubSystemHost sender)
        {
            host = sender;
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
