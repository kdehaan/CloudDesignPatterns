// <copyright file="IClient.cs" company="kdehaan">
// Copyright (c) kdehaan. All rights reserved.
// </copyright>

namespace CloudDesignPatterns.Client
{
    /// <summary>
    /// Interface for a basic client.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Connect to the server.
        /// </summary>
        void Connect();

        /// <summary>
        /// Send a message to the server.
        /// </summary>
        /// <param name="message">message to send.</param>
        void Send(string message);

        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        void Disconnect();
    }
}