using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CloudDesignPatterns
{
    public class ServerApp : IServer
    {
        private TcpListener? _listener;
        private bool _running = false;
        private readonly int _port;

        public ServerApp(int port)
        {
            _port = port;
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Loopback, _port);
            _listener.Start();
            _running = true;
            Console.WriteLine($"Server started on port {_port}.");

            while (_running)
            {
                if (_listener.Pending())
                {
                    var client = _listener.AcceptTcpClient();
                    var thread = new Thread(() => HandleClient(client));
                    thread.Start();
                }
                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            _running = false;
            _listener?.Stop();
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