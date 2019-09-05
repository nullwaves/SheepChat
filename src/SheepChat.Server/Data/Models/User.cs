using System;

namespace SheepChat.Server.Data.Models
{
    public enum UserRole
    {
        Banned = 0,
        Member = 20,
        Moderator = 60,
        Admin = 80,
        Sysadmin = 100
    }

    public class User : DocumentBase
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime Registered { get; set; }
        public DateTime LastLogin { get; set; }
        public UserRole UserRole { get; set; }
    }
}
