namespace SheepChat.Server.Data.Models
{
    /// <summary>
    /// A document record of a User-owned room.
    /// </summary>
    public class RoomRecord : DocumentBase
    {
        /// <summary>
        /// ID of the user that owns this room.
        /// </summary>
        public int OwnerUserID { get; set; }

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
