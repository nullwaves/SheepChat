namespace SheepChat.Server.Data
{
    public static class DocumentRepository<T> where T : DocumentBase, new()
    {
        public static void Save(T entity)
        {
            using (var repo = DataManager.OpenDocumentSession<T>())
            {
                repo.Upsert(entity);
            }
        }

        public static T Load(string id)
        {
            using (var repo = DataManager.OpenDocumentSession<T>())
            {
                return (T)repo.GetById(id);
            }
        }
    }
}
