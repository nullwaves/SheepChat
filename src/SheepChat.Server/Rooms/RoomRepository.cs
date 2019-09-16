using SheepChat.Server.Data.Models;
using SheepChat.Server.Data.Repositories;
using System.Collections.Generic;

namespace SheepChat.Server.Rooms
{
    /// <summary>
    /// Extended functions for loading User-owned rooms from the database
    /// </summary>
    public static class RoomRepository
    {
        /// <summary>
        /// Save a RoomRecord to storage.
        /// </summary>
        /// <param name="record">Room to save</param>
        public static void Save(RoomRecord record) => DocumentRepository<RoomRecord>.Save(record);

        /// <summary>
        /// Load a RoomRecord from storage.
        /// </summary>
        /// <param name="id">ID of the Room</param>
        /// <returns>RoomRecord with matching ID or null</returns>
        public static RoomRecord Load(int id) => DocumentRepository<RoomRecord>.Load(id);

        /// <summary>
        /// Retrieves an instantiated array of all user owned rooms.
        /// </summary>
        /// <returns>An <see cref="IRoom"/> array of all User-owned rooms</returns>
        public static IRoom[] GetAllUserOwnedRooms()
        {
            var rooms = new List<IRoom>();
            var records = DocumentRepository<RoomRecord>.LoadAll();
            foreach (var record in records)
            {
                var room = new UserOwnedRoom(record);
                rooms.Add(room);
            }

            return rooms.ToArray();
        }

        /// <summary>
        /// Save the data of a UserOwnedRoom object to the stoage.
        /// </summary>
        /// <param name="room">Room with data to save</param>
        public static void Save(UserOwnedRoom room) => DocumentRepository<RoomRecord>.Save(room.Data);
    }
}
