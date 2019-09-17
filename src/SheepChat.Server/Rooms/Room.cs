using SheepChat.Server.Sessions;
using System;
using System.Collections.Generic;

namespace SheepChat.Server.Rooms
{
    /// <summary>
    /// Abstracte base class and functionality for a room.
    /// </summary>
    public abstract class Room : IRoom
    {
        /// <summary>
        /// Room name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Room description.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Sessions connected to this room.
        /// </summary>
        public Dictionary<string, Session> Sessions { get; } = new Dictionary<string, Session>();

        /// <summary>
        /// Number of sessions connected to this room.
        /// </summary>
        public int UserCount => Sessions.Count;

        /// <summary>
        /// Join a session to this room.
        /// </summary>
        /// <param name="session">Session requesting to join.</param>
        public void Join(Session session)
        {
            if (Sessions.ContainsKey(session.ID))
            {
                session.Write("<#magenta>You are already in that room.");
            }
            else
            {
                lock (Sessions)
                {
                    Sessions.Add(session.ID, session);
                    session.Write("<#green>");
                    session.Write(Name + Environment.NewLine);
                    session.Write(Description + Environment.NewLine);
                    session.Write("<#white>");
                }
            }
        }

        /// <summary>
        /// Remove a session that left this room.
        /// </summary>
        /// <param name="session">Session that left.</param>
        public void Leave(Session session)
        {
            lock (Sessions)
            {
                if (Sessions.ContainsKey(session.ID))
                {
                    Sessions.Remove(session.ID);
                }
            }
        }

        /// <summary>
        /// Send a message to all users in this room.
        /// </summary>
        /// <param name="message">Message to send</param>
        public void Send(string message)
        {
            lock(Sessions)
            {
                foreach(Session s in Sessions.Values)
                {
                    s.Write($"[{Name}] {message}");
                }
            }
        }
    }
}
