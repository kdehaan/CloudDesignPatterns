// <copyright file="ServerApp.cs" company="kdehaan">
// Copyright (c) kdehaan. All rights reserved.
// </copyright>

namespace CloudDesignPatterns.Server
{
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// Base Server application.
    /// </summary>
    public class ServerApp : IServer
    {
        private readonly int port;
        private TcpListener? listener;
        private bool running = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerApp"/> class.
        /// </summary>
        /// <param name="port">listening port.</param>
        public ServerApp(int port)
        {
            this.port = port;
        }

        /// <inheritdoc/>
        public void Start()
        {
            this.listener = new TcpListener(IPAddress.Loopback, this.port);
            this.listener.Start();
            this.running = true;
            Console.WriteLine($"Server started on port {this.port}.");

            while (this.running)
            {
                if (this.listener.Pending())
                {
                    var client = this.listener.AcceptTcpClient();
                    var thread = new Thread(() => this.HandleClient(client));
                    thread.Start();
                }

                Thread.Sleep(100);
            }
        }

        /// <inheritdoc/>
        public void Stop()
        {
            this.running = false;
            this.listener?.Stop();
            Console.WriteLine("Server stopped.");
        }

        private void HandleClient(TcpClient client)
        {
            using var stream = client.GetStream();
            var buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                var received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {received}");
                var response = Encoding.UTF8.GetBytes($"Echo: {received}");
                stream.Write(response, 0, response.Length);
            }

            client.Close();
        }
    }
}