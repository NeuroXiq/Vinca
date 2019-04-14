using System;
using ProtocolEngine.Configuration.Filter;
using ProtocolEngine.ConnectionHandler;
using ProtocolEngine.Configuration.Server;
using Vinca.ProtocolEngine.Engine;
using Vinca.ProtocolEngine.Configuration.InternalEngineConfiguration;
using ProtocolEngine.Configuration.Parser;

namespace ProtocolEngine.Engine
{
    public class HttpEngine
    {
        public SocketHandler ConnectionHandler { get; private set; }

        HttpMarshal httpMarshal;

        private HttpEngine(SocketHandler handler)
        {
            ConnectionHandler = handler;
        }

        ///<summary>Initialize HTTP Engine and all components from configuration file</summary>
        public static HttpEngine Initialize()
        {
            string serverConfigFileName = InternalEngineConfiguration.DirectoryTree.ServerConfigurationFileRelativePath;
            string filterConfigFileName = InternalEngineConfiguration.DirectoryTree.FiltersConfigurationFileRelativePath;

            return Initialize(serverConfigFileName, filterConfigFileName);
        }
        
        private static HttpEngine Initialize(string serverConfigFileName, string filterConfigFileName)
        {
            FilterConfig filterConfig = GetFilterConfig(serverConfigFileName);
            ServerConfig serverConfig = GetServerConfig(filterConfigFileName);
            ConfigFileValidator.ThrowIfInvalidFilterConfig(filterConfig);
            ConfigFileValidator.ThrowIfInvalidServerConfig(serverConfig);

            EngineInitializationFactory initFactory = new EngineInitializationFactory(serverConfig, filterConfig);

            SocketHandler socketHanlder = initFactory.BuildSocketHandler();

            HttpEngine engine = new HttpEngine(socketHanlder);

            return engine;
        }

        private static ServerConfig GetServerConfig(string fileName)
        {
            return ConfigParser.ServerFromFile(fileName);
        }

        private static FilterConfig GetFilterConfig(string fileName)
        {
            return ConfigParser.FilterFromFile(fileName);
        }
    }
}
