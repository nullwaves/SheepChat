using SheepChat.Server.Sessions;
using System.Collections.Generic;

namespace SheepChat.Server.Rooms
{
    /// <summary>
    /// Interface for a room to allow the manager to load a variety of custom room types.
    /// </summary>
    public interface IRoom
    {
        /// <summary>
        /// Room Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Room Description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Sessions currently in the room.
        /// </summary>
        Dictionary<string, Session> Sessions { get; }

        /// <summary>
        /// Number of sessions currently in the room.
        /// </summary>
        int UserCount { get; }

        /// <summary>
        /// Join this session to the room.
        /// </summary>
        /// <param name="session">Session requesting to join.</param>
        void Join(Session session);

        /// <summary>
        /// Remove this session from the room.
        /// </summary>
        /// <param name="session">Session that left.</param>
        void Leave(Session session);

        /// <summary>
        /// Send a message to all users in the room.
        /// </summary>
        /// <param name="message">Message to be sent</param>
        void Send(string message);
    }
}