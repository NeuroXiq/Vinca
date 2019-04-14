using ProtocolEngine.MemoryManagement;

namespace Vinca.ProtocolEngine.Configuration.InternalEngineConfiguration
{
    internal static class InternalEngineConfiguration
    {
        public static class DirectoryTree
        {
            public const string ApacheMimeTypesFileRelativePath = "../Configuration/EngineResources/mime.types";
            public const string EngineResourcesRelativePath = "../Configuration/EngineResources";
            public const string ServerConfigurationFileRelativePath = "../Configuration/server.xml";
            public const string FiltersConfigurationFileRelativePath = "../Configuration/filters.xml";
        }

        public static class Info
        {
            public static string ServerName = "Vinca/0.8";
        }

        internal static class HttpEngineConfig
        {
            //TODO Extend this size
            public static int MaxHeaderLength = 123456;
        }

        internal static class BufferPoolConfig
        {
            public static BufferPool.BufferInfo[] BuffersInfo = new BufferPool.BufferInfo[]
            {
                new BufferPool.BufferInfo(100, 1  * 1024),
                new BufferPool.BufferInfo( 50, 2  * 1024),
                new BufferPool.BufferInfo( 20, 4  * 1024),
                new BufferPool.BufferInfo( 10, 8  * 1024),
                new BufferPool.BufferInfo(  5, 16 * 1024)
            };
        }
    }
}
