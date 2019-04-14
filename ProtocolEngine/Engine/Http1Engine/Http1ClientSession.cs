using ProtocolEngine.MemoryManagement;
using System;
using System.IO;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine
{
    public class Http1ClientSession : IDisposable
    {
        ///<summary>IO client stream</summary>
        public Stream Stream { get; private set; }
        ///<summary>Length of raw header bytes in <see cref="Buffer"/></summary>
        public int HeaderLengthInBuffer { get; private set; }
        ///<summary>Raw header bytes</summary>
        public DynamicBuffer Buffer { get; private set; }
        ///<summary>Parsed bytes holded by <see cref="Buffer"/></summary>
        public RequestHeader CurrentHeader { get; private set; }

        public Http1ClientSession(Stream ioStream)
        {
            Stream = ioStream;
            DynamicBuffer array = new DynamicBuffer();

            Buffer = new DynamicBuffer();
            HeaderLengthInBuffer = 0;
        }

        public void Dispose()
        {
            Buffer.Dispose();
        }

        public void UpdateHeader(RequestHeader header, int lengthInBuffer)
        {
            CurrentHeader = header;
            HeaderLengthInBuffer = lengthInBuffer;
        }
    }
}
