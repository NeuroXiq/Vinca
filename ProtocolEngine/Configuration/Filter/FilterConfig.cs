using ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation;
using System;

namespace ProtocolEngine.Configuration.Filter
{
    [Serializable]
    public class FilterConfig
    {
        public AuthenticationFilter Authentication;
        public DispositionFilter Disposition;
        public RedirectionFilter Redirection;
        public ForbiddenFilter Forbidden;
    }
}
