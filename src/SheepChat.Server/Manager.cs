using SheepChat.Server.Interfaces;

namespace SheepChat.Server
{
    public abstract class Manager : ISystem
    {
        public abstract string Name { get; }
        public ISystemHost SystemHost { get; private set; }

        protected readonly object Lock = new object();

        public abstract void Start();

        public abstract void Stop();

        public void SubscribeToSystemHost(ISystemHost host)
        {
            SystemHost = host;
        }

        public void UpdateSubSystemHost(ISubSystem sender, string msg)
        {
            SystemHost.UpdateSystemHost(this, msg);
        }
    }
}
