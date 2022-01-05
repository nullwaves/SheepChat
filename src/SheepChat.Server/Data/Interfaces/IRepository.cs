using System;
using System.Collections.Generic;

namespace SheepChat.Server.Data.Interfaces
{
    /// <summary>
    /// Interface for laying out a basic object repository
    /// </summary>
    /// <typeparam name="T">Class type extending the <see cref="IModel"/> interface.</typeparam>
    public interface IRepository<T> where T : IModel
    {
        /// <summary>
        /// Create instance <paramref name="entity"/> in repository.
        /// </summary>
        /// <param name="entity">Model to be created in the repository.</param>
        /// <returns>Created object.</returns>
        T Create(T entity);

        /// <summary>
        /// Fetch a single object from the repository.
        /// </summary>
        /// <param name="entity">Object to be fetched.</param>
        /// <returns>Fetched object.</returns>
        T Get(T entity);

        /// <summary>
        /// Get a filtered list of objects from the repository.
        /// </summary>
        /// <param name="where">LINQ expression to filter results.</param>
        /// <returns>Objects matching the expression.</returns>
        IEnumerable<T> Filter(Func<T, bool> where);
    }
}