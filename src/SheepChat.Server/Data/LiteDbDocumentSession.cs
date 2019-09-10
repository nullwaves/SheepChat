using LiteDB;
using System.Collections.Generic;

namespace SheepChat.Server.Data
{
    public class LiteDbDocumentSession<T> : IDocumentSession<T> where T : DocumentBase
    {
        private readonly LiteDatabase Database;
        private readonly LiteCollection<T> Collection;

        public LiteDbDocumentSession()
        {
            Database = new LiteDatabase(@"storage.ldb");
            Collection = Database.GetCollection<T>().IncludeAll();
        }

        public void Delete(T entity)
        {
            Collection.Delete(entity.ID);
        }

        public void Dispose()
        {
            Database.Dispose();
        }

        public T GetById(string id)
        {
            return Collection.FindById(id);
        }

        public void Insert(T entity)
        {
            Collection.Insert(entity);
        }

#pragma warning disable CS0693
        public IEnumerable<T> Query()
        {
            return Collection.FindAll();
        }
#pragma warning restore CS0693

        public void Update(T entity)
        {
            Collection.Update(entity);
        }

        public void Upsert(T entity)
        {
            Collection.Upsert(entity);
        }
    }
}
