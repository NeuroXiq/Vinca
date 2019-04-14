using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using System;
using System.IO;
using System.Text;
using ProtocolEngine.Http.Http1.Protocol;

namespace ProtocolEngine.HtdocsResourcesManagement.HtdocsSystem
{
    class HtdocsCacheSystem : IHtdocsSystem
    {
        const string ConfigFileName = ".CACHE_CONF";
        const string Version = "0.8";

        public struct CacheInfo
        {
            public int MinFileLength;
            public int MaxFileLength;
            public int MaxCacheSize;
        }

        PathResolver pathResolver;
        CacheInfo cacheInfo;
        string cacheDirectory;

        public HtdocsCacheSystem(string cacheDirectory, string htdocsRootDirectory, CacheInfo cacheInfo)
        {
            pathResolver = new PathResolver(htdocsRootDirectory);
            this.cacheInfo = cacheInfo;
            this.cacheDirectory = cacheDirectory;
        }


        ///<summary></summary>
        public static HtdocsCacheSystem Open(string directory, string htdocsRootDir)
        {
            return new HtdocsCacheSystem(directory, htdocsRootDir, CacheInfoFromConfig());
        }

        private static CacheInfo CacheInfoFromConfig()
        {
            return new CacheInfo()
            {
                MaxCacheSize = 100,
                MinFileLength = 1,
                MaxFileLength = 20
            };
        }

        public static HtdocsCacheSystem CreateNew(string directory,string htdocsDirectory, CacheInfo cacheInfo)
        {
            if (Directory.Exists(directory))
                throw new InvalidOperationException("Cannot create new cache system because directory with this name already exist");

            

            // Just write empty cache config to say, that this dir was opened by cache system before.
            // Currently, this config file do not contain any sensible data
            CreateConfigFile(directory, cacheInfo);

            return new HtdocsCacheSystem(directory, htdocsDirectory, cacheInfo);


        }

        private static void CreateConfigFile(string directory,CacheInfo cacheInfo)
        {
            Directory.CreateDirectory(directory);
            FileStream fstream = new FileStream(directory + ConfigFileName, FileMode.CreateNew);

            string cacheFile = "Cache system config file\n\r" +
                "Created      : " + DateTime.Now + "\r\n" +
                "Version      : " + Version + "\r\n" +
                "MinFileLength: " + cacheInfo.MinFileLength + "\r\n" +
                "MaxFileLength: " + cacheInfo.MaxFileLength + "\r\n" +
                "MaxCacheSize : " + cacheInfo.MaxCacheSize + "\r\n";

            byte[] bytes = Encoding.ASCII.GetBytes(cacheFile);

            fstream.Write(bytes, 0, bytes.Length);
            fstream.Close();
        }

        public bool FileExists(string relativePath)
        {
            throw new NotImplementedException();
        }

        public bool CanRangeStream(string relativePath)
        {
            throw new NotImplementedException();
        }

        public string GetETag(string fileName, EncodingType encoding)
        {
            throw new NotImplementedException();
        }

        public long GetLength(string relativePath, EncodingType encoding)
        {
            throw new NotImplementedException();
        }

        public EncodingType GetEnableEncodings(string relativePath)
        {
            throw new NotImplementedException();
        }

        public Stream OpenStream(string relativePath, EncodingType encoding)
        {
            throw new NotImplementedException();
        }

        public void CloseStream(Stream s)
        {
            throw new NotImplementedException();
        }
    }
}
