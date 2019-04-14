using System.IO;
using System.Net.Sockets;

namespace ProtocolEngine.ConnectionHandler
{
    abstract class HttpStreamMarshal
    {
        public abstract void ProcessHttpStream(Stream ioStream, Protocol protocol, Socket acceptedSocket);
    }
}
