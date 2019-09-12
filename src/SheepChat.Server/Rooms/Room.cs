using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;
using System;
using System.Collections.Generic;

namespace SheepChat.Server.Rooms
{
    public abstract class Room : IRoom
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public Dictionary<string, Session> Sessions { get; } = new Dictionary<string, Session>();

        public int UserCount => Sessions.Count;

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
                    session.Write(Name + Environment.NewLine);
                    session.Write(Description + Environment.NewLine);
                }
            }
        }

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

        public void Send(string message)
        {
            lock(Sessions)
            {
                foreach(Session s in Sessions.Values)
                {
                    s.Write(message);
                }
            }
        }
    }
}
