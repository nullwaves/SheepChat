using RepoDb;
using SheepChat.Server.Config;
using SheepChat.Server.Data.Interfaces;

namespace SheepChat.Server.Data.SQLite
{
    [ExportStorageProvider]
    internal class SQLiteStorageProvider : IStorageProvider
    {
        public string Name => "SQLite";

        public IRepository<T> OpenRepository<T>() where T : class, IModel
        {
            return new SQLiteRepository<T>(ConfigManager.Current.ConnectionString);
        }

        public void Prepare()
        {
        }
    }
}