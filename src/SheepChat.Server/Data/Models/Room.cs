using SheepChat.Server.Data.Interfaces;
using SheepChat.Server.Data.Managers;

namespace SheepChat.Server.Data.Models
{
    /// <summary>
    /// A document record of a User-owned room.
    /// </summary>
    public class Room : IModel
    {
        internal static ModelManager<Room> manager = new ModelManager<Room>();

        /// <summary>
        /// Room ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// ID of the user that owns this room.
        /// </summary>
        public long OwnerUserID { get; set; }

        /// <summary>
        /// Room Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Room Description
        /// </summary>
        public string Description { get; set; }
    }
}