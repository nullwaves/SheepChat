using System;
using System.Collections.Generic;
using System.Text;

namespace SheepChat.Server.Data
{
    class LiteDbDocumentStorageProvider : IDocumentStorageProvider
    {
        public string Name => "LiteDb";

        public IDocumentSession<T> OpenDocumentSession<T>()
        {
            return new LiteDbDocumentSession();
        }

        public void Prepare()
        {
            throw new NotImplementedException();
        }
    }
}
