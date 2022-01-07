namespace SheepChat.Server.Interfaces
{
    /// <summary>This is the most basic system.</summary>
    public interface ISystemBase
    {
        /// <summary>Starts this system.</summary>
        void Start();

        /// <summary>Stops this system.</summary>
        void Stop();
    }
}