using System;

namespace ProtocolEngine.Engine
{
    class HttpEngineInitializationException : Exception
    {
        public HttpEngineInitializationException(string message) : base(message)
        {
        }
    }
}
