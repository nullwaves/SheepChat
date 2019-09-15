namespace SheepChat.Server.Data
{
    /// <summary>
    /// Basic document repository class for quick saving and loading documents
    /// </summary>
    /// <typeparam name="T">Document type that inherits DocumentBase</typeparam>
    public static class DocumentRepository<T> where T : DocumentBase, new()
    {
        /// <summary>
        /// Save a document to it's respective collection. This is an ambiguous function that does not differentiate between new and existing documents.
        /// </summary>
        /// <param name="entity">Document to save.</param>
        public static void Save(T entity)
        {
            using (var repo = DataManager.OpenDocumentSession<T>())
            {
                repo.Upsert(entity);
            }
        }

        /// <summary>
        /// Load a document with a specified ID.
        /// </summary>
        /// <param name="id">ID of the document to load</param>
        /// <returns>Document with the supplied ID or null</returns>
        public static T Load(string id)
        {
            using (var repo = DataManager.OpenDocumentSession<T>())
            {
                return repo.GetById(id);
            }
        }
    }
}
