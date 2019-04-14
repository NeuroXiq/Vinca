using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using System;
using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation
{
    class Filter : IFilter
    {
        public bool Check(RequestHeader requestHeader, ResponseHeader response)
        {
            throw new NotImplementedException();
        }
    }
}
