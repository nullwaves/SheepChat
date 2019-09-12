using SheepChat.Server.Sessions;
using System.Collections.Generic;

namespace SheepChat.Server.Interfaces
{
    public interface IRoom
    {
        string Name { get; }
        string Description { get; }
        Dictionary<string, Session> Sessions { get; }
        int UserCount { get; }
        void Join(Session session);
        void Leave(Session session);
        void Send(string message);
    }
}
