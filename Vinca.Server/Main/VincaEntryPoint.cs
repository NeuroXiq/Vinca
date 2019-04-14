using ProtocolEngine.Engine;
using System;

namespace Vinca.Server.Main
{
    class VincaEntryPoint
    {
        /*
         * Vinca entry point. 
         * Application start here.
         */
        public static void Main(string[] args)
        {
            HttpEngine engine = HttpEngine.Initialize();
        }
    }
}
