using SheepChat.Server.Data.Models;
using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;
using System.Text;

namespace SheepChat.Server.Commands
{
    [ExportCommand("help")]
    public class HelpCommand : ICommand
    {
        public string Name => "Help";

        public string ShortDescription => "Displays information about available commands";

        public string HelpPage =>
            "Usage: $help [command]\r" +
            "\r" +
            "command     -Command to display help for\r" +
            "\r" +
            "Example: \"$help help\" displays this page as opposed to the commandlist\r";

        public CommandUseLevel UseLevel => CommandUseLevel.User;

        public UserRole MinimumUserRole => UserRole.Banned;

        public string DefaultHelpPrompt;

        public HelpCommand()
        {
            DefaultHelpPrompt = "";
        }

        public void Execute(Session sender, string[] args)
        {
            if (args.Length > 0)
            {
                var key = args[0];
                ICommand c = CommandManager.Instance.Dictionary.ContainsKey(key) ? CommandManager.Instance.Dictionary[key] : null;
                sender.Write(c == null ? c.HelpPage : "<#magenta>Could not find any information on \"$key\"\r<#white>");
            }
            else
            {
                sender.Write(DefaultHelpPrompt);
            }
        }

        public void Register()
        {
            // Build the default help prompt
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

            DefaultHelpPrompt = builder.ToString();
        }
    }
}