using SheepChat.Server.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SheepChat.Server.Data.SQLite
{
    internal class SQLiteRepository<T> : IRepository<T> where T : IModel
    {
        internal string _connectionString;

        public SQLiteRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public long Create(T entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Filter(Expression<Func<T, bool>> where)
        {
            throw new NotImplementedException();
        }

        public T Get(T entity)
        {
            throw new NotImplementedException();
        }

        public T Get(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public int Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}