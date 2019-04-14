using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using System;
using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation
{
    [Serializable]
    public class AuthenticationFilter : IFilter
    {
        public string UserName;
        public string Password;

        public bool Check(RequestHeader requestHeader, ResponseHeader response)
        {
            throw new NotImplementedException();
        }
    }
}
