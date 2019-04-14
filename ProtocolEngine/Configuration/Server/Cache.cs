using System;

namespace ProtocolEngine.Configuration.Server
{
    [Serializable]
    class Cache
    {
        public int MinFileLength;
        public int MaxFileLength;
        public int MaxCacheSize;
        public string Directory;
    }
}
