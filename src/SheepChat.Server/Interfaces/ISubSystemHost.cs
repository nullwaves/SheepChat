namespace SheepChat.Server.Interfaces
{
    /// <summary>An interface describing a SubSystemHost.</summary>
    public interface ISubSystemHost
    {
        /// <summary>Allows the sub system host to receive an update when subscribed to this system.</summary>
        /// <param name="sender">The sender of the update.</param>
        /// <param name="message">The message to send to the host.</param>
        void UpdateSubSystemHost(ISubSystem sender, string message);
    }
}