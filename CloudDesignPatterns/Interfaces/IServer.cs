// <copyright file="IServer.cs" company="kdehaan">
// Copyright (c) kdehaan. All rights reserved.
// </copyright>

namespace CloudDesignPatterns.Server
{
    /// <summary>
    /// Interface for a basic server.
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Start the server.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the server.
        /// </summary>
        void Stop();
    }
}