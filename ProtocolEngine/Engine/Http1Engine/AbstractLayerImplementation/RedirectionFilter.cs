using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using System;
using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;
using System.Text.RegularExpressions;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation
{
    [Serializable]
    public class RedirectionFilter : IPayloadIgnoreFilter
    {
        public string RegexPath;
        public string Location;
        public int Status;

        public bool Check(RequestHeader requestHeader, ResponseHeader response)
        {
            Regex regex = new Regex(RegexPath);
            if (regex.IsMatch(requestHeader.Target.Path))
            {
                response.StatusCode = (StatusCode)Status;
                response.Add(new LocationHf(Location));
                return false;
            }
            return true;
        }
    }
}
