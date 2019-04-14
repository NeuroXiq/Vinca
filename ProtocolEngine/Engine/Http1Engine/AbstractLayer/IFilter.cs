using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayer
{
    interface IFilter
    {
        bool Check(RequestHeader requestHeader, ResponseHeader response);
    }
}
