using SheepChat.Server.Data.Interfaces;

namespace SheepChat.Server.Data.LiteDb
{
    [ExportStorageProvider]
    internal class LiteDbStorageProvider : IStorageProvider
    {
        public string Name => "LiteDb";

        public IRepository<T> OpenRepository<T>() where T : class, IModel
        {
            return new LiteDbRepository<T>();
        }

        public void Prepare()
        {
        }
    }
}