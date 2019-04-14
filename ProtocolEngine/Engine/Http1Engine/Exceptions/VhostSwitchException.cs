using System;

namespace ProtocolEngine.Engine.Http1Engine
{
    class VhostSwitchException : Exception
    {
        public VhostSwitchException(string message) : base(message)
        {
        }
    }
}
