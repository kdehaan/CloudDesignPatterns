namespace CloudDesignPatterns
{
    public interface IClient
    {
        void Connect();
        void Send(string message);
        void Disconnect();
    }
}