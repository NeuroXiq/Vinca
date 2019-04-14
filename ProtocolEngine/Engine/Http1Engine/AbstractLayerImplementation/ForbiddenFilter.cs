using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using System;
using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation
{
    [Serializable]
    public class ForbiddenFilter : IFilter
    {
        public string RegexName;

        public bool Check(RequestHeader requestHeader, ResponseHeader response)
        {
            throw new NotImplementedException();
        }
    }
}
