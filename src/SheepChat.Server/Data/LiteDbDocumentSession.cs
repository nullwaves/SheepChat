using LiteDB;
using System.Collections.Generic;

namespace SheepChat.Server.Data
{
    /// <summary>
    /// A LiteDB wrapper for <see cref="IDocumentSession{T}"/>. Provides functionality for the system to get and store documents in a LiteDB NoSQL database.
    /// </summary>
    /// <typeparam name="T">Document type that inherits <see cref="DocumentBase"/></typeparam>
    public class LiteDbDocumentSession<T> : IDocumentSession<T> where T : DocumentBase
    {
        private readonly LiteDatabase Database;
        private readonly LiteCollection<T> Collection;

        /// <summary>
        /// Default constructor for a LiteDBDocumentSession that sets 
        /// </summary>
        public LiteDbDocumentSession()
        {
            Database = new LiteDatabase(@"storage.ldb");
            Collection = Database.GetCollection<T>().IncludeAll();
        }

        /// <summary>
        /// Delete a document from the collection.
        /// </summary>
        /// <param name="entity">Document to delete</param>
        public void Delete(T entity)
        {
            Collection.Delete(entity.ID);
        }

        /// <summary>
        /// Dispose of any disposable objects.
        /// </summary>
        public void Dispose()
        {
            Database.Dispose();
        }

        /// <summary>
        /// Get document by it's ID
        /// </summary>
        /// <param name="id">ID of the document to fetch</param>
        /// <returns>Document with the specified ID or null</returns>
        public T GetById(string id)
        {
            return Collection.FindById(id);
        }

        /// <summary>
        /// Insert a new document into the collection.
        /// </summary>
        /// <param name="entity">New document</param>
        public void Insert(T entity)
        {
            Collection.Insert(entity);
        }

        /// <summary>
        /// Queryable object for fetching more than one document, or complex searching.
        /// </summary>
        /// <returns>Queryable LiteCollection of all documents</returns>
        public IEnumerable<T> Query()
        {
            return Collection.FindAll();
        }

        /// <summary>
        /// Update an existing document in the collection.
        /// </summary>
        /// <param name="entity">Document to update.</param>
        public void Update(T entity)
        {
            Collection.Update(entity);
        }

        /// <summary>
        /// Insert or update a document of ambiguous status.
        /// </summary>
        /// <param name="entity">Document to upsert</param>
        public void Upsert(T entity)
        {
            Collection.Upsert(entity);
        }
    }
}
