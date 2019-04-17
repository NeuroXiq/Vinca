using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using System;
using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;
using System.Text.RegularExpressions;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation
{
    [Serializable]
    public class ForbiddenFilter : IPayloadIgnoreFilter
    {
        public string RegexName;

        public bool Check(RequestHeader requestHeader, ResponseHeader response)
        {
            Regex regex = new Regex(RegexName);
            if (regex.IsMatch(requestHeader.Target.Path))
            {
                response.StatusCode = StatusCode.Forbidden;
                return false;
            }
            return true;
        }
    }
}
