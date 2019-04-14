namespace ProtocolEngine.Http.Http1.Protocol
{
    public struct RequestTarget
    {
        public enum RequestTargetForm
        {
            Origin,
            Absolute,
            Authority,
            Asterisk
        }

        public RequestTargetForm Form;
        public string Scheme;
        public string UserInfo;
        public string Host;
        public int Port;
        public string Path;
        public string Query;
   }
}
