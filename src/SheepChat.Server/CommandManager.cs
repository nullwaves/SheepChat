using SheepChat.Server.Interfaces;
using SheepChat.Server.Sessions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SheepChat.Server
{
    /// <summary>
    /// Command manager system
    /// </summary>
    public class CommandManager : Manager, IRecomposable
    {
        /// <summary>
        /// Singleton instance to prevent duplicate systems.
        /// </summary>
        public static CommandManager Instance { get; } = new CommandManager();

        /// <summary>
        /// Manager system name
        /// </summary>
        public override string Name => "CommandSystem";

        /// <summary>
        /// Loosely imported command types
        /// </summary>
        [ImportMany]
        public Lazy<ICommand, ExportCommandAttribute>[] AvailableCommands { get; set; }

        /// <summary>
        /// Dictionary of commands in system.
        /// </summary>
        public readonly Dictionary<string, ICommand> Dictionary;

        /// <summary>
        /// String trigger that a client uses to tell the system it wants to try a command.
        /// </summary>
        public string Trigger { get; } = "$";

        /// <summary>
        /// Keywords that default to the user would like to disconnect.
        /// </summary>
        public static readonly string[] QuitKeywords = new string[]
        {
            "stop",
            "quit",
            "exit",
            "dc"
        };

        /// <summary>
        /// Private constructor to allow the singleton instance to be meaningful.
        /// </summary>
        private CommandManager()
        {
            Recompose();
            Dictionary = new Dictionary<string, ICommand>();
        }

        /// <summary>
        /// Compose any loose imports.
        /// </summary>
        public void Recompose()
        {
            Composer.Compose(this);
        }

        /// <summary>
        /// Start the Command system.
        /// </summary>
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

        /// <summary>
        /// Stopo the command system.
        /// </summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");

            Dictionary.Clear();

            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>
        /// Parse incoming data for a valid base command, then pass it on to the proper handler.
        /// </summary>
        /// <param name="sender">Session attempting to execute the command</param>
        /// <param name="command">Command trying to be executed</param>
        public void ProcessCommand(Session sender, string command)
        {
            var args = command.Split(' ').ToList();
            if (args.Count > 0)
            {
                string key = args[0].ToLower();
                if(QuitKeywords.Contains(key))
                {
                    sender.Write("<#green>Come back soon.", false);
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

    /// <summary>
    /// Singleton instance exporter for the Command manager system.
    /// </summary>
    [ExportInstance]
    public class CommandManagerInstance : InstanceExporter
    {
        /// <summary>
        /// Singleton instance of the CommandManager.
        /// </summary>
        public override ISystem Instance => CommandManager.Instance;

        /// <summary>
        /// Instance type of CommandManager.
        /// </summary>
        public override Type InstanceType => typeof(CommandManager);
    }
}
