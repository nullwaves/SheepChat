using System;

namespace SheepChat.Server.Data.Models
{
    /// <summary>
    /// Enum of User Roles used by the system.
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Banned users, should be kicked if authenticated.
        /// </summary>
        Banned = 0,

        /// <summary>
        /// Default member status.
        /// </summary>
        Member = 20,
        
        /// <summary>
        /// Member with more access and control of the server. Should help to keep rules enforced and acknowledged.
        /// </summary>
        Moderator = 60,

        /// <summary>
        /// Staff members, access to just about all commands except for any critical core server commands.
        /// </summary>
        Admin = 80,

        /// <summary>
        /// Sysadmin. Full access.
        /// </summary>
        Sysadmin = 100
    }

    /// <summary>
    /// User document that details the required data for a User
    /// </summary>
    public class User : DocumentBase
    {
        /// <summary>
        /// User's username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User's hashed password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// DateTime at which the user registered.
        /// </summary>
        public DateTime Registered { get; set; }

        /// <summary>
        /// DateTime at which the user last logged in.
        /// </summary>
        public DateTime LastLogin { get; set; }

        /// <summary>
        /// User's UserRole for permissions
        /// </summary>
        public UserRole UserRole { get; set; }
    }
}
