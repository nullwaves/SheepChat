using System.Collections.Generic;
using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;

namespace SheepChat.Server.Rooms
{
    public class DefaultRoom : Room
    {
        public override string Name => "Main";

        public override string Description => "The main lobby for the server. All users, by default, are deposited here upon logging in.";
    }
}
