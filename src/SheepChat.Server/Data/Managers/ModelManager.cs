using SheepChat.Server.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SheepChat.Server.Data.Managers
{
    /// <summary>
    /// Generic model manager
    /// </summary>
    /// <typeparam name="T">Model class that implements <see cref="IModel"/></typeparam>
    public class ModelManager<T> : IModelManager<T> where T : IModel
    {
        internal static readonly IRepository<T> repository = DataManager.OpenRepository<T>();

        /// <summary>
        /// Create model in database.
        /// </summary>
        /// <param name="entity">Model to add.</param>
        /// <returns>ID of added model.</returns>
        public long Create(T entity)
        {
            return repository.Create(entity);
        }

        /// <summary>
        /// Delete model from database.
        /// </summary>
        /// <param name="entity">Model to delete.</param>
        /// <returns>Number of deleted models.</returns>
        public int Delete(T entity)
        {
            return repository.Delete(entity);
        }

        /// <summary>
        /// Fetch a filtered list of objects from the database.
        /// </summary>
        /// <param name="where">LINQ Expression to filter by.</param>
        /// <returns>List of objects.</returns>
        public IEnumerable<T> Filter(Expression<Func<T, bool>> where)
        {
            return repository.Filter(where);
        }

        /// <summary>
        /// Get object from database by it's ID.
        /// </summary>
        /// <param name="id">ID of object to fetch.</param>
        /// <returns>Object with ID.</returns>
        public T Get(long id)
        {
            return repository.Get(id);
        }

        /// <summary>
        /// Fetch all objects of this type from the database.
        /// </summary>
        /// <returns>All objects.</returns>
        public IEnumerable<T> GetAll()
        {
            return repository.GetAll();
        }

        /// <summary>
        /// Save any updates to an object to the database.
        /// </summary>
        /// <param name="entity">Updated object.</param>
        /// <returns>Number of affected objects.</returns>
        public int Save(T entity)
        {
            return repository.Update(entity);
        }
    }
}