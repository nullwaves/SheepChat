namespace SheepChat.Server.Data.Interfaces
{
    /// <summary>
    /// Interface for a data storage provider.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Name of Storage Provider
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Prepare storage provider at system start-up.
        /// </summary>
        void Prepare();

        /// <summary>
        /// Get a repository for managing data objects.
        /// </summary>
        /// <typeparam name="T">Model </typeparam>
        /// <returns></returns>
        IRepository<T> OpenRepository<T>() where T : IModel;
    }
}