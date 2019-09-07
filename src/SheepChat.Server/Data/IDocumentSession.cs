using System;
using System.Collections.Generic;

namespace SheepChat.Server.Data
{
    public abstract class DocumentBase
    {
        public int ID { get; protected set; }
    }

    public interface IDocumentSession<T> : IDisposable where T : DocumentBase
    {
        /// <summary>
        /// Get document entity from storage by ID.
        /// </summary>
        /// <param name="id">ID of the document to get</param>
        /// <returns>Document or null</returns>
        T GetById(string id);

        /// <summary>
        /// Gets a queryable object for the repo.
        /// </summary>
        /// <typeparam name="T">Document Type</typeparam>
        /// <returns>IEnumerable<typeparamref name="T"/> object</returns>
#pragma warning disable CS0693
        IEnumerable<T> Query();
#pragma warning restore CS0693

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
        /// <param name="entity">Updated document/param>
        void Update(T entity);

        /// <summary>
        /// Ambiguous function to insert a document or update the document if it already exists.
        /// </summary>
        /// <param name="entity">Document to upsert</param>
        void Upsert(T entity);
    }
}
