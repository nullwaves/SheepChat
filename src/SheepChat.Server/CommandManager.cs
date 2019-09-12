using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SheepChat.Server
{
    public class CommandManager : Manager, IRecomposable
    {
        public static CommandManager Instance { get; } = new CommandManager();
        public override string Name => "CommandSystem";

        [ImportMany]
        public Lazy<ICommand, ExportCommandAttribute>[] AvailableCommands { get; set; }

        public readonly Dictionary<string, ICommand> Dictionary;

        public string Trigger { get; } = "$";

        public static readonly string[] QuitKeywords = new string[]
        {
            "stop",
            "quit",
            "dc"
        };

        public CommandManager()
        {
            Recompose();
            Dictionary = new Dictionary<string, ICommand>();
        }

        public void Recompose()
        {
            Composer.Compose(this);
        }

        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");

            if (AvailableCommands.Length > 0)
            {
                string[] uniqKeys = (from c in AvailableCommands select c.Metadata.Keyword).Distinct().ToArray();
                foreach (var key in uniqKeys)
                {
                    ICommand command = (from c in AvailableCommands
                                       where c.Metadata.Keyword == key
                                       orderby c.GetType().Assembly.GetName().Version
                                       select c.Value).FirstOrDefault();
                    Dictionary.Add(key, command);
                    command.Register();
                    SystemHost.UpdateSystemHost(this, "Registered command: " + key);
                }
            }

            SystemHost.UpdateSystemHost(this, "Started");
        }

        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");

            Dictionary.Clear();

            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        public void ProcessCommand(Session sender, string command)
        {
            var args = command.Split(' ').ToList();
            if (args.Count > 0)
            {
                string key = args[0].ToLower();
                if(QuitKeywords.Contains(key))
                {
                    sender.Write("<#lime>Come back soon.", false);
                    sender.Connection.Disconnect();
                    return;
                }
                ICommand c = Dictionary.ContainsKey(key) ? Dictionary[key] : null;
                c?.Execute(sender, args.GetRange(1, args.Count - 1).ToArray());
                if (c == null)
                {
                    sender.Write("<#magenta>Invalid Command<#white>" + Environment.NewLine);
                }
            }

        }
    }

    [ExportInstance]
    public class CommandManagerInstance : InstanceExporter
    {
        public override ISystem Instance => CommandManager.Instance;

        public override Type InstanceType => typeof(CommandManager);
    }
}
