using RepoDb;
using SheepChat.Server.Config;
using SheepChat.Server.Data.Interfaces;

namespace SheepChat.Server.Data.MySql
{
    [ExportStorageProvider]
    internal class MySqlStorageProvider : IStorageProvider
    {
        public string Name => "MySql";

        public IRepository<T> OpenRepository<T>() where T : class, IModel
        {
            return new MySqlRepository<T>(ConfigManager.Current.ConnectionString);
        }

        public void Prepare()
        {
        }
    }
}