using System;

namespace ProtocolEngine.Configuration.Server
{
    [Serializable]
    class ServerConfig
    {
        public int Port;
        public string HostName;
        public string RootDirectory;
        public Cache CacheConfiguration;
        public Tls TlsConfig;
    }
}
