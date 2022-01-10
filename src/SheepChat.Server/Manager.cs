using SheepChat.Server.Interfaces;

namespace SheepChat.Server
{
    /// <summary>
    /// Abstract Manager System that defines the basic and required functionality of a Manager system.
    /// </summary>
    public abstract class Manager : ISystem
    {
        /// <summary>
        /// Manager system name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// <see cref="ISystemHost"/> that is hosting this <see cref="ISystem"/>.
        /// </summary>
        public ISystemHost SystemHost { get; private set; }

        /// <summary>
        /// Lock object for blocking on unsafe object calls.
        /// </summary>
        protected readonly object Lock = new object();

        /// <summary>
        /// Start the manager system.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stop the manager system.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Subscribe to a <see cref="ISystemHost"/>
        /// </summary>
        /// <param name="host">Host to subscribe to</param>
        public void SubscribeToSystemHost(ISystemHost host)
        {
            SystemHost = host;
        }

        /// <summary>
        /// Pass on a message from a <see cref="ISubSystem"/> being hosted by this manager, to the manager's <see cref="ISystemHost"/>
        /// </summary>
        /// <param name="sender">Hosted <see cref="ISubSystem"/> sending the message</param>
        /// <param name="msg">Message being passed up the chain</param>
        public void UpdateSubSystemHost(ISubSystem sender, string msg)
        {
            SystemHost.UpdateSystemHost(this, msg);
        }
    }
}