using System;

namespace ProtocolEngine.Http.Http1.Parser
{
    public class RequestHeaderParserException : Exception
    {
        public RequestHeaderParserException(string message) : base(message)
        {
        }
    }
}
