using System;
using System.Collections.Generic;

namespace SheepChat.Server.Data
{
    /// <summary>
    /// Abstract base class for an object to be used as a Document with our storage provider
    /// </summary>
    public abstract class DocumentBase
    {
        /// <summary>
        /// Integer ID number of a document.
        /// </summary>
        public int ID { get; protected set; }
    }

    /// <summary>
    /// A basic document session interface to allow the use of any service implemented over a <see cref="IDocumentStorageProvider"/> that provides a wrapper for this interface.
    /// </summary>
    /// <typeparam name="T">Document type that inherits <see cref="DocumentBase"/></typeparam>
    public interface IDocumentSession<T> : IDisposable where T : DocumentBase
    {
        /// <summary>
        /// Get document entity from storage by ID.
        /// </summary>
        /// <param name="id">ID of the document to get</param>
        /// <returns>Document or null</returns>
        T GetById(int id);

        /// <summary>
        /// Gets a queryable object for the repo.
        /// </summary>
        /// <returns>IEnumerable<typeparamref name="T"/> object</returns>
        IEnumerable<T> Query();

        /// <summary>
        /// Insert a new document into storage.
        /// </summary>
        /// <param name="entity">Document to insert</param>
        void Insert(T entity);

        /// <summary>
        /// Delete a document from storage.
        /// </summary>
        /// <param name="entity">Document to delete</param>
        void Delete(T entity);

        /// <summary>
        /// Update a document already in storage.
        /// </summary>
        /// <param name="entity">Updated document</param>
        void Update(T entity);

        /// <summary>
        /// Ambiguous function to insert a document or update the document if it already exists.
        /// </summary>
        /// <param name="entity">Document to upsert</param>
        void Upsert(T entity);
    }
}
