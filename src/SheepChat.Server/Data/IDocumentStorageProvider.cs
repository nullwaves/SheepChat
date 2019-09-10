using System;
using System.ComponentModel.Composition;

namespace SheepChat.Server.Data
{
    public interface IDocumentStorageProvider
    {
        string Name { get; }
        void Prepare();
        IDocumentSession<T> OpenDocumentSession<T>() where T : DocumentBase;
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportDSPAttribute : ExportAttribute
    {
        public ExportDSPAttribute() : base(typeof(IDocumentStorageProvider))
        {
        }
    }
}
