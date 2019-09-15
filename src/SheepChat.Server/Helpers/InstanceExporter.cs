using SheepChat.Server.Interfaces;
using System;
using System.ComponentModel.Composition;

namespace SheepChat.Server
{
    /// <summary>
    /// Base class for an exported system.
    /// </summary>
    public abstract class InstanceExporter
    {
        /// <summary>
        /// Singleton instance of the System being exported.
        /// </summary>
        public abstract ISystem Instance { get; }

        /// <summary>
        /// Type of the System Instance being exported.
        /// </summary>
        public abstract Type InstanceType { get; }
    }

    /// <summary>
    /// Instance export attribute for tagging Systems that should be imported on system boot.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportInstanceAttribute : ExportAttribute
    {
        /// <summary>
        /// Default constructor for the instance export attribute with the appropriate contract.
        /// </summary>
        public ExportInstanceAttribute() : base(typeof(InstanceExporter))
        {
        }

    }
}
