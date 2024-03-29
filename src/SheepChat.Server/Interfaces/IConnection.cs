﻿using System.Net;

namespace SheepChat.Server.Interfaces
{
    /// <summary>
    /// Interface for the basic connection that allows all systems to interact with any wrapped connections.
    /// </summary>
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
        /// The last input termintaor used by the client.
        /// </summary>
        string LastInputTerminator { get; set; }

        /// <summary>
        /// The last raw input sent by the client.
        /// </summary>
        string LastRawInput { get; set; }

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
        /// Send data to the conneciton.
        /// </summary>
        /// <param name="data">The string to send.</param>
        /// <param name="bypassFormatter">Wheter or not the string should be formatted before being sent.</param>
        void Send(string data, bool bypassFormatter);

        /// <summary>
        /// Utilize buffer to produce output.
        /// </summary>
        void ProcessBuffer();
    }
}