using System;

namespace ProtocolEngine.Configuration.Server
{
    [Serializable]
    public class ServerConfig
    {
        public int Port;
        public string HostName;
        public string RootDirectory;
        public Cache CacheConfiguration;
        public Tls TlsConfig;
    }
}
