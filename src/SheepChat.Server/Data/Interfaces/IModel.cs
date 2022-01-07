namespace SheepChat.Server.Data.Interfaces
{
    /// <summary>
    /// Interface for a model object.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Model ID in database
        /// </summary>
        long ID { get; set; }
    }
}