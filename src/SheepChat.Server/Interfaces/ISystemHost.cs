namespace SheepChat.Server.Interfaces
{
    /// <summary>
    /// Interface for a class that can host a <see cref="ISystem"/>.
    /// </summary>
    public interface ISystemHost
    {
        /// <summary>
        /// Receiving an update from a hosted system.
        /// </summary>
        /// <param name="sender"><see cref="ISystem"/> sending the message</param>
        /// <param name="msg">Message being sent</param>
        void UpdateSystemHost(ISystem sender, string msg);
    }
}