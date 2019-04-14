using System;

namespace ProtocolEngine.Http1.Protocol.Exceptions
{
    public class HeaderFieldParseException : Exception
    {
        public HeaderFieldParseException(string message) : base(message)
        {
        }
    }
}
