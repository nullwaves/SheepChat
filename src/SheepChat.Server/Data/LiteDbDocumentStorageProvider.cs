namespace SheepChat.Server.Data
{
    [ExportDSP]
    class LiteDbDocumentStorageProvider : IDocumentStorageProvider
    {
        public string Name => "LiteDb";

        public IDocumentSession<T> OpenDocumentSession<T>() where T : DocumentBase
        {
            return new LiteDbDocumentSession<T>();
        }

        public void Prepare()
        {
        }
    }
}
