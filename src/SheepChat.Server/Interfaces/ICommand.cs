using SheepChat.Server.Data.Models;
using SheepChat.Server.Sessions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace SheepChat.Server.Interfaces
{
    public enum CommandUseLevel
    {
        User = 10,
        Room = 50,
        Server = 100
    }

    public interface ICommand
    {
        string Name { get; }
        CommandUseLevel UseLevel { get; }
        UserRole MinimumUserRole { get; }
        string ShortDescription { get; }
        string HelpPage { get; }
        void Register();
        void Execute(Session sender, string[] args);
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportCommandAttribute : ExportAttribute
    {
        public string Keyword { get; set; }
        public ExportCommandAttribute(string keyword) : base(typeof(ICommand))
        {
            Keyword = keyword.ToLower();
        }

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
