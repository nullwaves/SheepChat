namespace SheepChat.Server.Interfaces
{
    /// <summary>
    /// Interface for a System that hosts subsystem and can subscribe to a <see cref="ISystemHost"/>
    /// </summary>
    public interface ISystem : ISystemBase, ISubSystemHost
    {
        /// <summary>
        /// Subscribes this system to a host, as to receive updates.
        /// </summary>
        /// <param name="host"></param>
        void SubscribeToSystemHost(ISystemHost host);
    }
}