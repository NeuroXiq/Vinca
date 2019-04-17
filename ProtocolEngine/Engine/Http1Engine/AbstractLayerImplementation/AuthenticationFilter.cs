using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using System;
using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;
using System.Text.RegularExpressions;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation
{
    [Serializable]
    public class AuthenticationFilter : IPayloadIgnoreFilter
    {
        public string UserName;
        public string Password;
        public string PathRegexMatch;

        public bool Check(RequestHeader requestHeader, ResponseHeader response)
        {
            Regex r = new Regex(PathRegexMatch);

            if (r.IsMatch(requestHeader.Target.Path))
            {
                if (requestHeader.Contains(HFType.Authorization))
                {
                    AuthorizationHf authHf = requestHeader.GetSingleField<AuthorizationHf>(HFType.Authorization);
                    if (authHf.UserName == UserName && Password == authHf.Password)
                    {
                        return true;
                        
                    }
                }

                response.StatusCode = StatusCode.Unauthorized;
                response.Add(new WWWAuthenticateHf("authorization", "UTF-8"));
                    return false;
            } 
            else return true;
        }
    }
}
