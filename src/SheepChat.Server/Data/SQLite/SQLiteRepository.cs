using RepoDb;
using SheepChat.Server.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;

namespace SheepChat.Server.Data.SQLite
{
    internal class SQLiteRepository<T> : IRepository<T> where T : class, IModel
    {
        internal string _connectionString;

        /// <summary>
        /// SQLite wrapped Model repository.
        /// </summary>
        /// <param name="connectionString">Connection string to SQLite database file.</param>
        public SQLiteRepository(string connectionString)
        {
            _connectionString = connectionString;
            if (!SQLiteBootstrap.IsInitialized) SQLiteBootstrap.Initialize();
        }

        public long Create(T entity)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    return (long)connection.Insert(entity);
                }
                catch
                {
                    return -1;
                }
            }
        }

        public int Delete(T entity)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    return connection.Delete(entity);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public void Dispose()
        {
        }

        public IEnumerable<T> Filter(Expression<Func<T, bool>> where)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    return connection.Query(where);
                }
                catch
                {
                    return new List<T>();
                }
            }
        }

        public T Get(T entity)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    return connection.Query<T>(entity).FirstOrDefault();
                }
                catch
                {
                    return null;
                }
            }
        }

        public T Get(long id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    return connection.Query<T>(id).FirstOrDefault();
                }
                catch
                {
                    return null;
                }
            }
        }

        public IEnumerable<T> GetAll()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    return connection.QueryAll<T>();
                }
                catch
                {
                    return null;
                }
            }
        }

        public int Update(T entity)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    return connection.Update<T>(entity);
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}