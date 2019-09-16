using SheepChat.Server.Data.Repositories;
using SheepChat.Server.Data.Models;
using SheepChat.Server.Interfaces;
using System.Collections.Generic;

namespace SheepChat.Server.Rooms
{
    /// <summary>
    /// Room Manager System for loading and handling user events while chatting.
    /// </summary>
    public class RoomManager : Manager, IRecomposable
    {
        /// <summary>
        /// Singleton instance so there is only ever one given RoomManager.
        /// </summary>
        public static RoomManager Instance { get; } = new RoomManager();

        /// <summary>
        /// Manager System Name
        /// </summary>
        public override string Name => "Rooms";

        /// <summary>
        /// Dictionary of Rooms and their reference string.
        /// </summary>
        public static Dictionary<string, IRoom> RoomList { get; private set; }

        /// <summary>
        /// Compose any loose imports.
        /// </summary>
        public void Recompose()
        {
            Composer.Compose(this);
        }

        /// <summary>
        /// Start the Room system.
        /// </summary>
        public override void Start()
        {
            RoomList.Add("main", new DefaultRoom());
            LoadUserOwnedRooms();
        }

        private void LoadUserOwnedRooms()
        {
            var records = DocumentRepository<RoomRecord>.LoadAll();
            foreach (var record in records)
            {
                var room = new UserOwnedRoom(record);
                RoomList.Add(record.Name.ToLower(), room);
            }
        }

        /// <summary>
        /// Stop the Room system.
        /// </summary>
        public override void Stop()
        {
            throw new System.NotImplementedException();
        }

        private RoomManager()
        {
            Recompose();
        }
    }
}
