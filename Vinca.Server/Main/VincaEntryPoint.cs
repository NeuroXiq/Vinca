using ProtocolEngine.Engine;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Vinca.Server.Main
{
    class VincaEntryPoint
    {
        /*
         * Vinca entry point. 
         * Application start here.
         *
         * * * */

        static HttpEngine httpEngine;

        public static void Main(string[] args)
        {
            HttpEngine engine = HttpEngine.Initialize();
            httpEngine = engine;
            Start();
        }

        public static void Start()
        {
            //test listener
            Socket listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, httpEngine.ConnectionHandler.Port));
            listener.Listen(50);
            while (true)
            {
                Socket accepted = listener.Accept();

                Task.Factory.StartNew(() => 
                {
                    httpEngine.ConnectionHandler.AcceptSocket(accepted);
                });
            }

        }
    }
}
