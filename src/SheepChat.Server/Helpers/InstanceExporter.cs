using SheepChat.Server.Interfaces;
using System;
using System.ComponentModel.Composition;

namespace SheepChat.Server
{
    public abstract class InstanceExporter
    {
        public abstract ISystem Instance { get; }

        public abstract Type InstanceType { get; }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportInstanceAttribute : ExportAttribute
    {
        public ExportInstanceAttribute() : base(typeof(InstanceExporter))
        {
        }

    }
}
