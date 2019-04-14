using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace ProtocolEngine.ConnectionHandler
{
    class SslHandler : SocketHandler
    {
        private HttpStreamMarshal streamMarshal;
        X509Certificate2 x509Certificate;

        public SslHandler(int port,HttpStreamMarshal streamMarshal, X509Certificate2 x509Certificate) : base(port)
        {
            this.x509Certificate = x509Certificate;
            this.streamMarshal = streamMarshal;
        }

        public override void AcceptSocket(Socket socket)
        {
            NetworkStream encryptedStream = new NetworkStream(socket);
            SslStream sslStream = new SslStream(encryptedStream);
            sslStream.AuthenticateAsServer(x509Certificate);

            streamMarshal.ProcessHttpStream(sslStream,Protocol.Http11, socket);
        }
    }
}
