using SheepChat.Server.Data.Models;
using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;
using SheepChat.Server.SessionStates;
using System;

namespace SheepChat.Server.Commands
{
    [ExportCommand("me")]
    public class MeCommand : ICommand
    {
        public string Name => "Me";

        public CommandUseLevel UseLevel => CommandUseLevel.User;

        public UserRole MinimumUserRole => UserRole.Member;

        public string ShortDescription => "It's tradition.";

        public string HelpPage => 
            "Usage: $me [action]\r" +
            "\r" +
            "You're gunna need a bigger shield.";

        public void Execute(Session sender, string[] args)
        {
            var users = ChattingState.Who.Values;
            var msg = string.Join(" ", args);
            var cyan = ANSI.Color(ANSI.FGColorBit.Cyan);
            var white = ANSI.Color(ANSI.FGColorBit.White);
            foreach(var user in users)
            {
                user.Write($"{cyan}{sender.User.Username} {msg}{white}{Environment.NewLine}", true);
            }
        }

        public void Register()
        {
        }
    }
}
