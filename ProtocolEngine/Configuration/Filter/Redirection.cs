using System;

namespace ProtocolEngine.Configuration.Filter
{
    [Serializable]
    class Redirection
    {
        public string PathNameRegex;
        public string Location;
        public int StatusCode;
    }
}
