using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using System;
using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation
{
    class StaticInjectFilter : IFieldInjectFilter
    {

        private IHeaderField[] headerFields;
           
        public StaticInjectFilter(IHeaderField[] fields)
        {
            headerFields = fields;
        }

        public void InjectField(RequestHeader requestHeader, ResponseHeader responseSession)
        {
            responseSession.Add(headerFields);
        }
    }
}
