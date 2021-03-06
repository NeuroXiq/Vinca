﻿using System;
using System.IO;
using System.Net.Sockets;
using ProtocolEngine.ConnectionHandler;
using ProtocolEngine.Engine.Http1Engine;
using ProtocolEngine;

namespace Vinca.ProtocolEngine.Engine
{
    class HttpMarshal : HttpStreamMarshal
    {
        Interpreter h1Interpreter;


        public HttpMarshal(Interpreter h1Interpreter)
        {
            this.h1Interpreter = h1Interpreter;
        }

        public override void ProcessHttpStream(Stream ioStream, Protocol protocol, Socket acceptedSocket)
        {
            if (protocol == Protocol.Http11)
            {
                Session s = new Session(ioStream);
                try
                {
                    h1Interpreter.Start(s);
                }
                catch (Exception e)
                {
                    GlobalConsoleDebug.ShowInternalError(e);
                    
                }
                
                // BuildHttp1Session(ioStream);
                // http1interpreter.Start(session);
                //
            }
            else if (protocol == Protocol.H2)
            {
                //buildh2session
                //h2interpreter.start(h2sssion)
                //
            }
            else throw new NotSupportedException("protocol");
        }
    }
}
