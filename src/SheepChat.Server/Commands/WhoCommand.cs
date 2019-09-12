using SheepChat.Server.Data.Models;
using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;
using SheepChat.Server.SessionStates;
using System.Linq;
using System.Text;

namespace SheepChat.Server.Commands
{
    [ExportCommand("who")]
    public class WhoCommand : ICommand
    {
        public string Name => "Who";

        public CommandUseLevel UseLevel => CommandUseLevel.Room;

        public UserRole MinimumUserRole => UserRole.Member;

        public string ShortDescription => "Lists all users in the current room.";

        public string HelpPage =>
            "Usage: $who\r" +
            "\r" +
            "Lists all users in the current room.\r";

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

        public void Register()
        {
        }
    }
}
