using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayer
{
    interface IFieldInjectFilter
    {
        void InjectField(RequestHeader requestHeader, ResponseHeader responseSession);
    }
}
