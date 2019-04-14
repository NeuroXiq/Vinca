using ProtocolEngine.Http.Http1.Parser;
using ProtocolEngine.MemoryManagement;
using System.IO;
using Vinca.ProtocolEngine.Configuration.InternalEngineConfiguration;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine
{
    public class Session
    {
        static RequestHeaderParser headerParser = new RequestHeaderParser();
        static MessageStreamReader headerReader = new MessageStreamReader(InternalEngineConfiguration.HttpEngineConfig.MaxHeaderLength);

        public RequestHeader CurrentHeader { get; private set; }


        Stream ioStream;
        DynamicBuffer buffer;
        int headerLength = 0;

        public Session(Stream stream)
        {
            ioStream = stream;
            buffer = new DynamicBuffer();
        }

        internal void TakeNextHeader()
        {
            buffer.ResetTo(0, headerLength);
            headerLength = headerReader.ReadNextHeader(ioStream, buffer);
            CurrentHeader = headerParser.Parse(buffer.BufferArray, 0, headerLength);

        }

        public void Send(byte[] buffer, int offset, int count)
        {
            ioStream.Write(buffer, offset, count);
        }

        public void CopyStream(Stream source)
        {
            //source.CopyTo(ioStream);
            //source.Flush();
            //return;
            byte[] buf = new byte[3000000];
            int readed = 0;
            while ((readed = source.Read(buf, 0, 3000000)) > 0)
            {
                ioStream.Write(buf, 0, readed);
            }
        }
    }
}
