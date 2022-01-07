namespace SheepChat.Server.Interfaces
{
    /// <summary>
    /// An interface describing a SubSystem.
    /// </summary>
    public interface ISubSystem : ISystemBase
    {
        /// <summary>
        /// Subscribe to receive system updates from this system.
        /// </summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        void SubscribeToSystem(ISubSystemHost sender);

        /// <summary>
        /// Unsubscribe from currently subscribed system.
        /// </summary>
        void UnsubscribeToSystem();

        /// <summary>
        /// Inform subscribed system(s) of the specified update.
        /// </summary>
        /// <param name="msg">The message to be sent to subscribed system(s).</param>
        void InformSubscribedSystem(string msg);
    }
}