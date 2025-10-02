// <copyright file="BaseServerApp.cs" company="kdehaan">
// Copyright (c) kdehaan. All rights reserved.
// </copyright>

namespace CloudDesignPatterns.BaseComponents
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using CloudDesignPatterns.Server;

    /// <summary>
    /// Base Server application.
    /// </summary>
    public class BaseServerApp : IServer
    {
        private readonly int port;
        private TcpListener? listener;
        private bool running = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseServerApp"/> class.
        /// </summary>
        /// <param name="port">listening port.</param>
        public BaseServerApp(int port)
        {
            this.port = port;
            this.Endpoints = new Dictionary<string, Func<string, string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "ping", _ => CreateResponse(HttpStatusCode.OK, "pong") },
                { "echo", payload => CreateResponse(HttpStatusCode.OK, payload) },
            };
        }

        /// <summary>
        /// Gets or sets the endpoints that the server supports.
        /// </summary>
        protected Dictionary<string, Func<string, string>> Endpoints { get; set; }

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

        /// <summary>
        /// Constructs a standard response message.
        /// </summary>
        /// <param name="status">HTTP status code.</param>
        /// <param name="message">Message.</param>
        /// <returns>Formatted response.</returns>
        protected static string CreateResponse(HttpStatusCode status, string message)
        {
            return $"{(int)status} {status}: {message}";
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

                string responseStr = this.HandleRequest(received);
                var response = Encoding.UTF8.GetBytes(responseStr);
                stream.Write(response, 0, response.Length);
            }

            client.Close();
        }

        private string HandleRequest(string request)
        {
            // Expecting format: endpoint/payload
            var idx = request.IndexOf('/');
            if (idx < 0)
            {
                return "Invalid request format. Use endpoint/payload";
            }

            var endpoint = request.Substring(0, idx).Trim();
            var payload = request.Substring(idx + 1);

            if (this.Endpoints.TryGetValue(endpoint, out var handler))
            {
                return handler(payload);
            }
            else
            {
                return $"Unknown endpoint: {endpoint}";
            }
        }
    }
}