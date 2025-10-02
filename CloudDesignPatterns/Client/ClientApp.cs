// <copyright file="ClientApp.cs" company="kdehaan">
// Copyright (c) kdehaan. All rights reserved.
// </copyright>

namespace CloudDesignPatterns.Client
{
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// Base Client application.
    /// </summary>
    public class ClientApp : IClient
    {
        private readonly string host;
        private readonly int port;
        private TcpClient? client;
        private NetworkStream? stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientApp"/> class.
        /// </summary>
        /// <param name="host">host address.</param>
        /// <param name="port">port.</param>
        public ClientApp(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        /// <inheritdoc/>
        public void Connect()
        {
            this.client = new TcpClient();
            this.client.Connect(this.host, this.port);
            this.stream = this.client.GetStream();
            Console.WriteLine("Connected to server.");
        }

        /// <inheritdoc/>
        public void Send(string message)
        {
            if (this.stream == null)
            {
                return;
            }

            var data = Encoding.UTF8.GetBytes(message);
            this.stream.Write(data, 0, data.Length);

            var buffer = new byte[1024];
            int bytesRead = this.stream.Read(buffer, 0, buffer.Length);
            var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Server response: {response}");
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            this.stream?.Close();
            this.client?.Close();
            Console.WriteLine("Disconnected from server.");
        }
    }
}