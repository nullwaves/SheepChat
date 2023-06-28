/*using LiteDB;
using SheepChat.Server.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SheepChat.Server.Data.LiteDb
{
    /// <summary>
    /// A LiteDB wrapper for <see cref="IRepository{T}"/>. Provides functionality for the system to get and store documents in a LiteDB NoSQL database.
    /// </summary>
    /// <typeparam name="T">Document type that inherits <see cref="IModel"/></typeparam>
    public class LiteDbRepository<T> : IRepository<T> where T : class, IModel
    {
        private readonly LiteDatabase Database;
        private readonly LiteCollection<T> Collection;

        /// <summary>
        /// Default constructor for a LiteDBDocumentSession that sets
        /// </summary>
        public LiteDbRepository()
        {
            Database = new LiteDatabase(@"storage.ldb");
            Collection = Database.GetCollection<T>().IncludeAll();
        }

        /// <summary>
        /// Delete a document from the collection.
        /// </summary>
        /// <param name="entity">Document to delete</param>
        /// <returns>Number of deleted documents.</returns>
        public int Delete(T entity)
        {
            return Collection.Delete(x => x.Equals(entity));
        }

        /// <summary>
        /// Dispose of any disposable objects.
        /// </summary>
        public void Dispose()
        {
            Database.Dispose();
        }

        /// <summary>
        /// Fetch object from collection.
        /// </summary>
        /// <param name="entity">Object to fetch.</param>
        /// <returns>Fetched object.</returns>
        public T Get(T entity)
        {
            return Collection.Find(x => x.Equals(entity)).FirstOrDefault();
        }

        /// <summary>
        /// Get document by it's ID
        /// </summary>
        /// <param name="id">ID of the document to fetch</param>
        /// <returns>Document with the specified ID or null</returns>
        public T Get(long id)
        {
            return Collection.FindById(id);
        }

        /// <summary>
        /// Filter documents by LINQ expression.
        /// </summary>
        /// <param name="where">LINQ Expression to filter with.</param>
        /// <returns>Filtered documents.</returns>
        public IEnumerable<T> Filter(Expression<Func<T, bool>> where)
        {
            return Collection.Find(where);
        }

        /// <summary>
        /// Insert a new document into the collection.
        /// </summary>
        /// <param name="entity">New document</param>
        public long Create(T entity)
        {
            return Collection.Insert(entity);
        }

        /// <summary>
        /// Queryable object for fetching more than one document, or complex searching.
        /// </summary>
        /// <returns>Queryable LiteCollection of all documents</returns>
        public IEnumerable<T> GetAll()
        {
            return Collection.FindAll();
        }

        /// <summary>
        /// Update an existing document in the collection.
        /// </summary>
        /// <param name="entity">Document to update.</param>
        public int Update(T entity)
        {
            return Collection.Update(entity) ? 1 : 0;
        }
    }
}*/