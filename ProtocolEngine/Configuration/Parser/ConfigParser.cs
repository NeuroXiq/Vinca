using ProtocolEngine.Configuration.Filter;
using ProtocolEngine.Configuration.Server;
using System.IO;
using System.Xml.Serialization;

namespace ProtocolEngine.Configuration.Parser
{
    class ConfigParser
    {
        public static ServerConfig ServerFromFile(string fileName)
        {
            ServerConfig serverConfig = XmlDeserialize<ServerConfig>(fileName);

            return serverConfig;
        }

        public static FilterConfig FilterFromFile(string fileName)
        {
            return XmlDeserialize<FilterConfig>(fileName);
        }

        private static T XmlDeserialize<T>(string fileName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            return (T)xmlSerializer.Deserialize(new FileStream(fileName, FileMode.Open));
        }
    }
}
