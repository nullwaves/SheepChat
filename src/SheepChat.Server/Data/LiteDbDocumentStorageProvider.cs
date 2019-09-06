using LiteDB;
using System;

namespace SheepChat.Server.Data
{
    class LiteDbDocumentStorageProvider : IDocumentStorageProvider
    {
        public string Name => "LiteDb";

        public IDocumentSession<T> OpenDocumentSession<T>() where T : DocumentBase
        {
            return new LiteDbDocumentSession<T>();
        }

        public void Prepare()
        {
            throw new NotImplementedException();
        }
    }
}
