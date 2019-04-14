using System;

namespace ProtocolEngine.Configuration.Server
{
    [Serializable]
    public class Cache
    {
        public int MinFileLength;
        public int MaxFileLength;
        public int MaxCacheSize;
        public string Directory;
    }
}
