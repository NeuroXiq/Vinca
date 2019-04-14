using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation
{
    [Serializable]
    public class RedirectionFilter : IFilter
    {
        public string RegexPath;
        public string Location;
        public int StatusCode;

        public bool Check(RequestHeader requestHeader, ResponseHeader response)
        {
            throw new NotImplementedException();
        }
    }
}
