using SheepChat.Server.Data.Models;
using SheepChat.Server.Interfaces;
using SheepChat.Server.Rooms;
using SheepChat.Server.Sessions;
using System;

namespace SheepChat.Server.Commands
{
    /// <summary>
    /// Command for interacting with the room system
    /// </summary>
    [ExportCommand("room")]
    public class RoomCommand : ICommand
    {
        /// <summary>
        /// Command name
        /// </summary>
        public string Name => "Room";

        /// <summary>
        /// Level at which the command affects the systems
        /// </summary>
        public CommandUseLevel UseLevel => CommandUseLevel.User;

        /// <summary>
        /// Minimum required UserRole the user must have to execute this command
        /// </summary>
        public UserRole MinimumUserRole => UserRole.Member;

        /// <summary>
        /// Short description of the command
        /// </summary>
        public string ShortDescription => "Move around and access room functions";

        /// <summary>
        /// Help page definition for this command including usage and examples
        /// </summary>
        public string HelpPage =>
            $"Usage: $room [join] [new] name{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"join    -Move to another room{Environment.NewLine}" +
            $"new     -Create a new room{Environment.NewLine}" +
            $"name    -Name of a room{Environment.NewLine}" +
            $"{Environment.NewLine}" +
            $"Example: \"$room join main\" will move you to the main lobby.{Environment.NewLine}" +
            $"         \"$room new Bob'sBurgers\" will create a new room (provided Bob'sBurgers doesn't already exist){Environment.NewLine}";

        /// <summary>
        /// Attempt to execute this command
        /// </summary>
        /// <param name="sender">Session attempting to execute this command</param>
        /// <param name="args">Command parameters</param>
        public void Execute(Session sender, string[] args)
        {
            if (args.Length < 1)
            {
                var room = RoomManager.Instance.GetRoomContainingSession(sender);
                sender.Write($"<#green>{room.Name}{Environment.NewLine}{room.Description}{Environment.NewLine}<#white>");
                return;
            }
            switch(args[0].ToLower())
            {
                case "join":
                    if(args.Length > 1)
                    {
                        var name = args[1].ToLower();
                        if(RoomManager.RoomList.ContainsKey(name))
                        {
                            RoomManager.MoveTo(sender, name);
                        }
                        else
                        {
                            sender.Write($"<#magenta>The room \"{name}\" does not exist.<#white>{Environment.NewLine}");
                        }
                    }
                    else
                    {
                        sender.Write($"<#magenta>Must specify which room to join.<#white>{Environment.NewLine}");
                    }
                    return;
                case "new":
                    if(args.Length > 1)
                    {
                        var name = args[1];
                        var newRoom = RoomManager.CreateRoom(sender, name);
                        if(newRoom != null)
                        {
                            RoomManager.MoveTo(sender, newRoom.Name);
                        }
                        else
                        {
                            sender.Write($"<#magenta>Room with that name already exists.<#white>{Environment.NewLine}");
                        }
                    }
                    else
                    {
                        sender.Write($"<#magenta>Must declare a name for the new room.<#white>{Environment.NewLine}");
                    }
                    return;
                case "desc":
                    var newDesc = args.Length > 1 ? string.Join(" ", args, 1, args.Length-1) : string.Empty;
                    var thisRoom = RoomManager.Instance.GetRoomContainingSession(sender);
                    if(thisRoom.GetType() == typeof(UserOwnedRoom))
                    {
                        var userRoom = (UserOwnedRoom)thisRoom;
                        if(sender.User.ID == userRoom.Owner.ID)
                        {
                            userRoom.SetDescription(newDesc);
                            userRoom.Send($"{sender.User.Username} has changed this room's description.{Environment.NewLine}<#green>{userRoom.Name}{Environment.NewLine}{userRoom.Description}<#white>{Environment.NewLine}");
                            return;
                        }
                    }
                    sender.Write($"<#magenta>You cannot modify the description of this room.<#white>{Environment.NewLine}");
                    return;
                default:
                    if (RoomManager.RoomList.ContainsKey(args[0].ToLower()))
                    {
                        var room = RoomManager.RoomList[args[0].ToLower()];
                        sender.Write($"<#green>{room.Name}{Environment.NewLine}{room.Description}{Environment.NewLine}<#white>");
                    }
                    else
                    {
                        sender.Write($"<#magenta>Invalid command.<#white>{Environment.NewLine}");
                    }
                    return;
            }
        }

        /// <summary>
        /// Run any prep at system boot.
        /// </summary>
        public void Register()
        {
        }
    }
}
