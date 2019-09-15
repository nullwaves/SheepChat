using SheepChat.Server.Data.Models;
using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;
using SheepChat.Server.SessionStates;
using System.Linq;
using System.Text;

namespace SheepChat.Server.Commands
{
    /// <summary>
    /// User-list command, shows who's in the chatting state
    /// </summary>
    [ExportCommand("who")]
    public class WhoCommand : ICommand
    {
        /// <summary>
        /// Command name
        /// </summary>
        public string Name => "Who";

        /// <summary>
        /// The level at which this command affects the systems.
        /// </summary>
        public CommandUseLevel UseLevel => CommandUseLevel.Room;

        /// <summary>
        /// The minium user role required to run this command.
        /// </summary>
        public UserRole MinimumUserRole => UserRole.Member;

        /// <summary>
        /// Short description of this command.
        /// </summary>
        public string ShortDescription => "Lists all users in the current room.";

        /// <summary>
        /// Help page detailing usage of this command.
        /// </summary>
        public string HelpPage =>
            "Usage: $who\r" +
            "\r" +
            "Lists all users in the current room.\r";

        /// <summary>
        /// Execute this command.
        /// </summary>
        /// <param name="sender">Session attempting to execute this command</param>
        /// <param name="args">Command parameters</param>
        public void Execute(Session sender, string[] args)
        {
            var users = (from s in ChattingState.Who.Values
                         select s.User).ToArray();
            var activeCount = users.Length;
            var prompt = new StringBuilder();
            prompt.AppendLine($"<#cyan>{activeCount} Currently Available Users:");
            prompt.AppendLine();
            
            foreach(var user in users)
            {
                var line = string.Format(
                    "{0}- {1}",
                    user.ID.ToString().PadRight(5),
                    user.Username
                    );
                prompt.AppendLine(line);
            }
            prompt.AppendLine("<#white>");
            sender.Write(prompt.ToString());
        }

        /// <summary>
        /// Run any prep at system boot.
        /// </summary>
        public void Register()
        {
        }
    }
}
