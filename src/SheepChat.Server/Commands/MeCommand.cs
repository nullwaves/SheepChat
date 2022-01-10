using SheepChat.Server.Data.Models;
using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;
using SheepChat.Server.SessionStates;
using System;

namespace SheepChat.Server.Commands
{
    /// <summary>
    /// Role-playing/action command
    /// </summary>
    [ExportCommand("me")]
    public class MeCommand : ICommand
    {
        /// <summary>
        /// Command name
        /// </summary>
        public string Name => "Me";

        /// <summary>
        /// The level at which this command affects the systems.
        /// </summary>
        public CommandUseLevel UseLevel => CommandUseLevel.User;

        /// <summary>
        /// The minimum user role required to use this command.
        /// </summary>
        public UserRole MinimumUserRole => UserRole.Member;

        /// <summary>
        /// Short description of this command.
        /// </summary>
        public string ShortDescription => "It's tradition.";

        /// <summary>
        /// The help page detailing usage for this command.
        /// </summary>
        public string HelpPage =>
            "Usage: $me [action]\r" +
            "\r" +
            "You're gunna need a bigger shield.";

        /// <summary>
        /// Execute this command.
        /// </summary>
        /// <param name="sender">Session attempting to execute this command</param>
        /// <param name="args">Command parameters</param>
        public void Execute(Session sender, string[] args)
        {
            var users = ChattingState.Who.Values;
            var msg = string.Join(" ", args);
            var cyan = ANSI.Color(ANSI.FGColorBit.Cyan);
            var white = ANSI.Color(ANSI.FGColorBit.White);
            foreach (var user in users)
            {
                user.Write($"{cyan}{sender.User.Username} {msg}{white}{Environment.NewLine}", true);
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