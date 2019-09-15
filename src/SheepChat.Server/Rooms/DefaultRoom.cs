using System.Collections.Generic;
using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;

namespace SheepChat.Server.Rooms
{
    /// <summary>
    /// Default lobby room.
    /// </summary>
    public class DefaultRoom : Room
    {
        /// <summary>
        /// Room name
        /// </summary>
        public override string Name => "Main";

        /// <summary>
        /// Room Description
        /// </summary>
        public override string Description => "The main lobby for the server. All users, by default, are deposited here upon logging in.";
    }
}
