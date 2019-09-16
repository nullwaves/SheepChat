using SheepChat.Server.Data.Models;
using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;
using System;
using System.Text;

namespace SheepChat.Server.Commands
{
    /// <summary>
    /// The second most useful command in history aside from $kill
    /// </summary>
    [ExportCommand("help")]
    public class HelpCommand : ICommand
    {
        /// <summary>
        /// Command name
        /// </summary>
        public string Name => "Help";

        /// <summary>
        /// Short description of the help command.
        /// </summary>
        public string ShortDescription => "Displays information about available commands";

        /// <summary>
        /// Full help description of this command.
        /// </summary>
        public string HelpPage =>
            "Usage: $help [command]\r" +
            "\r" +
            "command     -Command to display help for\r" +
            "\r" +
            "Example: \"$help help\" displays this page as opposed to the commandlist\r";

        /// <summary>
        /// The level at which this command affects the systems.
        /// </summary>
        public CommandUseLevel UseLevel => CommandUseLevel.User;

        /// <summary>
        /// Minimum required <see cref="UserRole"/> to use this command.
        /// </summary>
        public UserRole MinimumUserRole => UserRole.Banned;

        /// <summary>
        /// Cached default help prompt
        /// </summary>
        public string DefaultHelpPrompt;

        /// <summary>
        /// Execute this command
        /// </summary>
        /// <param name="sender">Session attempting to execute this command</param>
        /// <param name="args">Command paramaters</param>
        public void Execute(Session sender, string[] args)
        {
            if (args.Length > 0)
            {
                var key = args[0];
                ICommand c = CommandManager.Instance.Dictionary.ContainsKey(key) ? CommandManager.Instance.Dictionary[key] : null;
                sender.Write(c != null ? c.HelpPage : $"<#magenta>Could not find any information on \"{key}\"\r<#white>" + Environment.NewLine);
            }
            else
            {
                DefaultHelpPrompt = DefaultHelpPrompt != string.Empty ? DefaultHelpPrompt : BuildDefaultHelpPrompt();
                sender.Write(DefaultHelpPrompt);
            }
        }

        /// <summary>
        /// Run any prep at system boot
        /// </summary>
        public void Register()
        {
            DefaultHelpPrompt = string.Empty;
        }

        /// <summary>
        /// Generates a default help prompt that lists all commands available on the server.
        /// </summary>
        /// <returns>The generated help prompt</returns>
        private string BuildDefaultHelpPrompt()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("The following commands are currently available:");
            builder.AppendLine();
            foreach (var kvp in CommandManager.Instance.Dictionary)
            {
                var line = string.Format("{0}-{1}", kvp.Key.PadRight(12), kvp.Value.ShortDescription);
                builder.AppendLine(line);
            }
            builder.AppendLine();
            builder.AppendLine("For more help, try $help followed by the command you'd like to know more about");
            builder.AppendLine();

            return builder.ToString();
        }
    }
}