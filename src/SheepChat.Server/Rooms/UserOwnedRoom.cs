﻿using SheepChat.Server.Data.Models;
using SheepChat.Server.Data.Repositories;

namespace SheepChat.Server.Rooms
{
    /// <summary>
    /// A user owned/managed room.
    /// </summary>
    public class UserOwnedRoom : Room
    {
        private RoomRecord Data;

        /// <summary>
        /// User that owns this room.
        /// </summary>
        public User Owner => UserRepository.Load(Data.OwnerUserID);

        /// <summary>
        /// Room name.
        /// </summary>
        public override string Name => Data.Name;

        /// <summary>
        /// Room Description.
        /// </summary>
        public override string Description => Data.Description;

        /// <summary>
        /// Set the name of the room
        /// </summary>
        /// <param name="newName">New name of the room</param>
        public void SetName(string newName)
        {
            Data.Name = newName.Trim();
        }

        /// <summary>
        /// Set the description of the room.
        /// </summary>
        /// <param name="newDescription">The new description of the room.</param>
        public void SetDescription(string newDescription)
        {
            Data.Description = newDescription.Trim();
        }
    }
}
