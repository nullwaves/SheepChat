using SheepChat.Server.Data.Models;
using SheepChat.Server.Sessions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace SheepChat.Server.Interfaces
{
    /// <summary>
    /// Enum of Command Use Levels that direct the <see cref="CommandManager"/> to which level the command should be executed at.
    /// </summary>
    public enum CommandUseLevel
    {
        /// <summary>
        /// Commands specific to and only affecting user(s)
        /// </summary>
        User = 10,

        /// <summary>
        /// Commands that affect or are executed only within the bounds of the current Room context
        /// </summary>
        Room = 50,

        /// <summary>
        /// Commands that affect the entire server, or large subsections of the server.
        /// </summary>
        Server = 100
    }

    /// <summary>
    /// Interface for a command class that can be used and managed by the <see cref="CommandManager"/>
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Command name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Level at which this command affects the systems
        /// </summary>
        CommandUseLevel UseLevel { get; }

        /// <summary>
        /// Minimum UserRole required to execute this command
        /// </summary>
        UserRole MinimumUserRole { get; }

        /// <summary>
        /// Short description of the command.
        /// </summary>
        string ShortDescription { get; }

        /// <summary>
        /// Detailed description of the command including usage and an example.
        /// </summary>
        string HelpPage { get; }

        /// <summary>
        /// Run any preperations on system boot. This would be where you should load any external data required for execution.
        /// </summary>
        void Register();

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="sender">Session attempting to execute this command</param>
        /// <param name="args">Command parameters</param>
        void Execute(Session sender, string[] args);
    }

    /// <summary>
    /// Command Export attribute for tagging Commands to be loaded on recomposition.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportCommandAttribute : ExportAttribute
    {
        /// <summary>
        /// Command keyword that is how the user references the command.
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Constructor for the Export Command attribute, setting the command's keyword.
        /// </summary>
        /// <param name="keyword"></param>
        public ExportCommandAttribute(string keyword) : base(typeof(ICommand))
        {
            Keyword = keyword.ToLower();
        }

        /// <summary>
        /// Constructor for importing metadata along with commands
        /// </summary>
        /// <param name="metadata"></param>
        public ExportCommandAttribute(IDictionary<string, object> metadata)
        {
            foreach (var key in metadata.Keys)
            {
                if (key == "Keyword")
                {
                    Keyword = (string)metadata[key];
                }
            }
        }
    }
}