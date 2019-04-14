using System;
using System.Net.Sockets;

namespace ProtocolEngine.ConnectionHandler
{
    class TcpSocketHandler : SocketHandler
    {
        HttpStreamMarshal streamMarshal;

        public TcpSocketHandler(int port, HttpStreamMarshal streamMarshal) : base(port)
        {
            this.streamMarshal = streamMarshal;
        }

        public override void AcceptSocket(Socket socket)
        {
            NetworkStream networkStream = new NetworkStream(socket);
            streamMarshal.ProcessHttpStream(networkStream, Protocol.Http11, socket);
        }
    }
}
