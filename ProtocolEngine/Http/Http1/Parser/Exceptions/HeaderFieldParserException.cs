using System;

namespace ProtocolEngine.Http.Http1.Parser
{
    public class HeaderFieldParserException : Exception
    {
        public HeaderFieldParserException(string message) : base(message)
        {
        }
    }
}
