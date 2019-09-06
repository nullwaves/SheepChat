using System;

namespace SheepChat.Server.Data
{
    public interface IDocumentStorageProvider
    {
        string Name { get; }
        void Prepare();
        IDocumentSession<T> OpenDocumentSession<T>() where T : DocumentBase;
    }
}
