using ProtocolEngine.Http.Http1.Protocol;

namespace Vinca.ProtocolEngine.Http1.Protocol
{
    public interface IHeaderField
    {
        HFType Type { get; }
    }
}
