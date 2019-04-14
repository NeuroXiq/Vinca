namespace ProtocolEngine.Http.Http1.Protocol
{
    public struct HttpVersion
    {
        public int Minor { get; private set; }
        public int Major { get; private set; }

        public HttpVersion(int major, int minor)
        {
            Minor = minor;
            Major = major;
        }
    }
}
