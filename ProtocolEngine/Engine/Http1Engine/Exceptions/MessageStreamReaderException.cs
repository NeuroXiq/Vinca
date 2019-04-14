using System;

namespace ProtocolEngine.Engine.Http1Engine
{
    public class MessageStreamReaderException : Exception
    {
        public MessageStreamReaderException(string message) : base(message)
        {
        }
    }
}
