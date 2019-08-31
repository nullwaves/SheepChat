namespace SheepChat.Server.Interfaces
{
    public interface ISystem : ISystemBase, ISubSystemHost
    {
        /// <summary>
        /// Subscribes this system to a host, as to receive updates.
        /// </summary>
        /// <param name="host"></param>
        void SubscribeToSystemHost(ISystemHost host);
    }
}
