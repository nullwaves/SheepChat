using System.Net;
using System.Text;

namespace SheepChat.Server.Interfaces
{
    public interface IConnection
    {
        /// <summary>
        /// Gets the ID of the connection.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Gets or sets the IPAddress for the connection.
        /// </summary>
        IPAddress CurrentIPAddress { get; }

        /// <summary>
        /// Gets the byte data waiting to be processed
        /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// Gets the textual representation of the data still waiting to be returned as an input string.
        /// </summary>
        StringBuilder Buffer { get; }

        /// <summary>
        /// Disconnect the connection.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Send data to the connection.
        /// </summary>
        /// <param name="data">The byte array to send.</param>
        void Send(byte[] data);

        /// <summary>
        /// Send data to the connection.
        /// </summary>
        /// <param name="data">The string to send.</param>
        void Send(string data);

        /// <summary>
        /// Utilize buffer to produce output.
        /// </summary>
        void ProcessBuffer();
    }
}
