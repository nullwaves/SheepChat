using System;
using System.ComponentModel.Composition;

namespace SheepChat.Server.Data
{
    /// <summary>
    /// A basic interface for a Document Storage Provider.
    /// </summary>
    public interface IDocumentStorageProvider
    {
        /// <summary>
        /// Document Storage Provider name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Run any prep at system boot.
        /// </summary>
        void Prepare();

        /// <summary>
        /// Open a new document session from this storage provider.
        /// </summary>
        /// <typeparam name="T">Document tupe that inherits DocumentBase</typeparam>
        /// <returns>A wrapped <see cref="IDocumentSession{T}"/> for this storage provider.</returns>
        IDocumentSession<T> OpenDocumentSession<T>() where T : DocumentBase;
    }

    /// <summary>
    /// Export attribute for Document Storage Providers
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportDSPAttribute : ExportAttribute
    {
        /// <summary>
        /// Default constructor for type contract
        /// </summary>
        public ExportDSPAttribute() : base(typeof(IDocumentStorageProvider))
        {
        }
    }
}
