using ProtocolEngine.Http.Http1.Parser;
using ProtocolEngine.MemoryManagement;
using System;
using System.IO;
using Vinca.ProtocolEngine.Configuration.InternalEngineConfiguration;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine
{
    public class Http1Session : IDisposable
    {
        //TODO  max header length should be given in other way
        private static MessageStreamReader messageReader = new MessageStreamReader(InternalEngineConfiguration.HttpEngineConfig.MaxHeaderLength);
        //shared parser
        private static RequestHeaderParser headerParser = new RequestHeaderParser();

        public Http1ClientSession ClientSession { get; private set; }

        public Http1Session(Stream s)
        {
            ClientSession = new Http1ClientSession(s);
        }

        public void Dispose()
        {
            ClientSession.Dispose();
        }

        ///<summary>Read next header from stream and update <see cref="Http1ClientSession"/> object</summary>
        public void UpdateToNextHeader()
        {
            //1. remove previous header bytes from buffer and shift all remainig to left.
            // Buffer can contain data that exceed length of current header (HTTP Pipelining)
            int curHeaderLen = ClientSession.HeaderLengthInBuffer;
            DynamicBuffer dBuffer = ClientSession.Buffer;
            dBuffer.ResetTo(0, curHeaderLen);

            //2. try find next header in buffer or append bytes from stream to buffer until '\r\n\r\n'
            Stream stream = ClientSession.Stream;
            int headerLength = messageReader.ReadNextHeader(stream, dBuffer);

            //parse readed bytes
            RequestHeader newHeader = headerParser.Parse(dBuffer.BufferArray, 0, headerLength);

            //update client session
            ClientSession.UpdateHeader(newHeader, headerLength);

        }
    }
}
