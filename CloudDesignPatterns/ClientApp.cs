using System.Net.Sockets;
using System.Text;

namespace CloudDesignPatterns
{
    public class ClientApp : IClient
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private readonly string _host;
        private readonly int _port;

        public ClientApp(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Connect()
        {
            _client = new TcpClient();
            _client.Connect(_host, _port);
            _stream = _client.GetStream();
            Console.WriteLine("Connected to server.");
        }

        public void Send(string message)
        {
            if (_stream == null) return;
            var data = Encoding.UTF8.GetBytes(message);
            _stream.Write(data, 0, data.Length);

            var buffer = new byte[1024];
            int bytesRead = _stream.Read(buffer, 0, buffer.Length);
            var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Server response: {response}");
        }

        public void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
            Console.WriteLine("Disconnected from server.");
        }
    }
}