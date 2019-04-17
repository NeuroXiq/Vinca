using System;
using ProtocolEngine.Configuration.Filter;
using ProtocolEngine.Configuration.Server;
using ProtocolEngine.ConnectionHandler;
using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using ProtocolEngine.HtdocsResourcesManagement;
using Vinca.ProtocolEngine.Configuration.InternalEngineConfiguration;
using ProtocolEngine.HtdocsResourcesManagement.HtdocsSystem;
using System.Security.Cryptography.X509Certificates;
using Vinca.ProtocolEngine.Engine;
using ProtocolEngine.Engine.Http1Engine;
using System.IO;
using ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation;
using System.Collections.Generic;

namespace ProtocolEngine.Engine
{
    class EngineInitializationFactory
    {
        MimeMapper mimeMapper;
        PathResolver pathResolver;
        HttpStreamMarshal httpStreamMarshal;


        // configuration file translated to objects.
        // this files are used to create server components
        ServerConfig serverConfig;
        FilterConfig filterConfig;

        public EngineInitializationFactory(ServerConfig serverConfig, FilterConfig filterConfig)
        {
            this.serverConfig = serverConfig;
            this.filterConfig = filterConfig;

            pathResolver = new PathResolver(serverConfig.RootDirectory);
            mimeMapper = MimeMapper.FromApacheFile(InternalEngineConfiguration.DirectoryTree.ApacheMimeTypesFileRelativePath, pathResolver);
            httpStreamMarshal = BuildHttpStreamMarshal();
        }

        public HttpStreamMarshal BuildHttpStreamMarshal()
        {
            Interpreter interpreter = new Interpreter(BuildHtdocsSystem(), BuildInjectFilters(), BuildPayloadIgnoreFilters(), mimeMapper);

            HttpMarshal marshal = new HttpMarshal(interpreter);

            return marshal;
        }

        private IFieldInjectFilter[] BuildInjectFilters()
        {
            return new IFieldInjectFilter[] { filterConfig.Disposition };
        }

        private IPayloadIgnoreFilter[] BuildPayloadIgnoreFilters()
        {
            return new IPayloadIgnoreFilter[]
            {
                filterConfig.Authentication,
                filterConfig.Forbidden,
                filterConfig.Redirection
            };
        }

        public SocketHandler BuildSocketHandler()
        {
            if (serverConfig.TlsConfig != null)
                return BuildSslSocketHandler();
            else return BuildTcpSocketHandler();
        }

        private SocketHandler BuildTcpSocketHandler()
        {
            SocketHandler handler = new TcpSocketHandler(serverConfig.Port, httpStreamMarshal);
            return handler;
        }

        private SocketHandler BuildSslSocketHandler()
        {
            X509Certificate2 x509Cert = new X509Certificate2(serverConfig.TlsConfig.Path, serverConfig.TlsConfig.Password);
            SslHandler handler = new SslHandler(serverConfig.Port, httpStreamMarshal, x509Cert);

            return handler;
        }

        internal IHtdocsSystem BuildHtdocsSystem()
        {
            if (serverConfig.CacheConfiguration == null)
            {
                return new HtdocsDefaultSystem(pathResolver);
                //return new HtdocsDefaultSystem
            }
            else
            {
                return CreateHtdocsCacheSystem();
            }
        }

        private IHtdocsSystem CreateHtdocsCacheSystem()
        {
            if (Directory.Exists(serverConfig.CacheConfiguration.Directory))
            {
                return HtdocsCacheSystem.Open(serverConfig.CacheConfiguration.Directory, serverConfig.RootDirectory);
            }
            else
            {
                HtdocsCacheSystem.CacheInfo cinfo = new HtdocsCacheSystem.CacheInfo();
                cinfo.MaxCacheSize = serverConfig.CacheConfiguration.MaxCacheSize;
                cinfo.MaxFileLength = serverConfig.CacheConfiguration.MaxFileLength;
                cinfo.MinFileLength = serverConfig.CacheConfiguration.MinFileLength;

                return HtdocsCacheSystem.CreateNew(serverConfig.CacheConfiguration.Directory, serverConfig.RootDirectory, cinfo);
            }
        }
    }
}
