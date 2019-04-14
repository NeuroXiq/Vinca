using System;
using System.IO;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine
{
    public class Session
    {
        public RequestHeader CurrentHeader { get; private set; }

        public Session(Stream stream)
        {

        }

        internal void TakeNextHeader()
        {
            throw new NotImplementedException();
        }
    }
}
