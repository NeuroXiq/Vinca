using System.Net.Sockets;

namespace ProtocolEngine.ConnectionHandler
{
    public abstract class SocketHandler
    {
        ///<summary>Indicates port associated with this connection handler</summary>
        public int Port { get; private set; }

        public SocketHandler(int port)
        {
            Port = port;
        }

        public abstract void AcceptSocket(Socket socket);
    }
}
