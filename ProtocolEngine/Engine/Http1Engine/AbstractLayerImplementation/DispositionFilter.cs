using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using System;
using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;
using System.Text.RegularExpressions;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation
{
    [Serializable]
    public class DispositionFilter : IFieldInjectFilter
    {
        public string RegexFileName;

        public void InjectField(RequestHeader requestHeader, ResponseHeader responseSession)
        {
            Regex r = new Regex(RegexFileName);
            string path = requestHeader.Target.Path;
            if (r.IsMatch(path))
            {
                Match m = r.Match(path);
                responseSession.Add(new UndefinedHf("Content-Disposition", "attachment; filename=\"" + path.Replace('/', '_') + "\""));
            }
        }
    }
}
