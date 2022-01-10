using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SheepChat.Server.Data.Interfaces
{
    /// <summary>
    /// Interface for a model manager.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModelManager<T> where T : IModel
    {
        /// <summary>
        /// Create instance <paramref name="entity"/> and save it.
        /// </summary>
        /// <param name="entity">Object to save</param>
        /// <returns>ID of created object.</returns>
        long Create(T entity);

        /// <summary>
        /// Fetch a object by its ID.
        /// </summary>
        /// <param name="id">ID of the object to fetch.</param>
        /// <returns>Fetched object.</returns>
        T Get(long id);

        /// <summary>
        /// Fetch all objects.
        /// </summary>
        /// <returns>Enumerable of all stored objects.</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Get a filtered list of objects.
        /// </summary>
        /// <param name="where">LINQ expression to filter results.</param>
        /// <returns>Objects matching the expression.</returns>
        IEnumerable<T> Filter(Expression<Func<T, bool>> where);

        /// <summary>
        /// Update object.
        /// </summary>
        /// <param name="entity">Object to be updated.</param>
        /// <returns>Number of updated rows.</returns>
        int Save(T entity);

        /// <summary>
        /// Delete entity from repository.
        /// </summary>
        /// <param name="entity">Entity to be deleted.</param>
        /// <returns>Number of deleted rows.</returns>
        int Delete(T entity);
    }
}