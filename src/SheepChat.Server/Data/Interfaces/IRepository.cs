using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SheepChat.Server.Data.Interfaces
{
    /// <summary>
    /// Interface for laying out a basic object repository
    /// </summary>
    /// <typeparam name="T">Class type extending the <see cref="IModel"/> interface.</typeparam>
    public interface IRepository<T> : IDisposable where T : class, IModel
    {
        /// <summary>
        /// Create instance <paramref name="entity"/> in repository.
        /// </summary>
        /// <param name="entity">Model to be created in the repository.</param>
        /// <returns>Created object.</returns>
        long Create(T entity);

        /// <summary>
        /// Fetch a single object from the repository.
        /// </summary>
        /// <param name="entity">Object to be fetched.</param>
        /// <returns>Fetched object.</returns>
        T Get(T entity);

        /// <summary>
        /// Fetch a single object by its ID.
        /// </summary>
        /// <param name="id">ID of the object to fetch.</param>
        /// <returns>Fetched object.</returns>
        T Get(long id);

        /// <summary>
        /// Fetch all objects from the repository.
        /// </summary>
        /// <returns>Enumerable of all stored objects.</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Get a filtered list of objects from the repository.
        /// </summary>
        /// <param name="where">LINQ expression to filter results.</param>
        /// <returns>Objects matching the expression.</returns>
        IEnumerable<T> Filter(Expression<Func<T, bool>> where);

        /// <summary>
        /// Update entity in repository.
        /// </summary>
        /// <param name="entity">Entity to be updated.</param>
        /// <returns>Number of updated rows.</returns>
        int Update(T entity);

        /// <summary>
        /// Delete entity from repository.
        /// </summary>
        /// <param name="entity">Entity to be deleted.</param>
        /// <returns>Number of deleted rows.</returns>
        int Delete(T entity);
    }
}