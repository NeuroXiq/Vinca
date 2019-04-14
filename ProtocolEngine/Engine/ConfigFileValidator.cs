using ProtocolEngine.Configuration.Filter;
using ProtocolEngine.Configuration.Server;

namespace ProtocolEngine.Engine
{
    class ConfigFileValidator
    {
        public static void ThrowIfInvalidServerConfig(ServerConfig serverConfig)
        {
            if (string.IsNullOrEmpty(serverConfig.HostName))
                throw new HttpEngineInitializationException("Invalid value of Host name");
            if (serverConfig.Port < 1 || serverConfig.Port > ushort.MaxValue)
                throw new HttpEngineInitializationException("Invalid value of Port");
            if (string.IsNullOrEmpty(serverConfig.RootDirectory))
                throw new HttpEngineInitializationException("Empty root directory");

            if (serverConfig.CacheConfiguration != null)
            {
                var cache = serverConfig.CacheConfiguration;
                if (cache.MaxCacheSize < 0)
                    throw new HttpEngineInitializationException("Cache size must be non-negative value");
                if (cache.MaxFileLength < 0)
                    throw new HttpEngineInitializationException("Cache max file length must be non-negative value");
                if (cache.MinFileLength < 0)
                    throw new HttpEngineInitializationException("Cache min file length must be non-negative value");
            }

            

        }

        public static void ThrowIfInvalidFilterConfig(FilterConfig filterConfig)
        {

        }
    }
}
