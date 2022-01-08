using RepoDb;
using SheepChat.Server.Data.Interfaces;

namespace SheepChat.Server.Data.SQLite
{
    [ExportStorageProvider]
    internal class SQLiteStorageProvider : IStorageProvider
    {
        public string Name => "SQLite";

        public IRepository<T> OpenRepository<T>() where T : IModel
        {
            return new SQLiteRepository<T>();
        }

        public void Prepare()
        {
        }
    }
}