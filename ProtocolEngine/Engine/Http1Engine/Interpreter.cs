using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using ProtocolEngine.HtdocsResourcesManagement;
using ProtocolEngine.Http.Http1.Protocol;
using System;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Engine.Http1Engine
{
    class Interpreter
    {
        IHtdocsSystem htdocsSystem;

        IFilter[] customFilters;
        IFilter[] constantFilters;
        MimeMapper mimeMapper;

        public Interpreter(IHtdocsSystem htdocsSystem, IFilter[] customFilters, MimeMapper mimeMapper)
        {
            this.customFilters = customFilters;
            this.mimeMapper = mimeMapper;
        }


        public void Start(Session session)
        {
            while (KeepConnection(session))
            {
                session.TakeNextHeader();
                RequestHeader requestHeader = session.CurrentHeader;
                ResponseHeader responseSession = new ResponseHeader();


                bool sendPayload = Filter(requestHeader, responseSession);
                
                AddCommonResponseFields(responseSession);
                SendHeader(session, responseSession);

                if (sendPayload)
                    SendPayload(session, responseSession);
            }
        }

        private bool Filter(RequestHeader requestHeader, ResponseHeader responseSession)
        {
            bool sendPayload = false;



            return sendPayload;
        }

        private void SendPayload(Session requestHeader, ResponseHeader responseSession)
        {
            throw new NotImplementedException();
        }

        private void SendHeader(Session session, ResponseHeader responseSession)
        {
            throw new NotImplementedException();
        }

        private bool FileSystemFilter(RequestHeader requestHeader, ResponseHeader responseSession)
        {
            string relativePath = requestHeader.Target.Path;

            if (!htdocsSystem.FileExists(relativePath))
            {

                return false;
            }

            return false;
        }

        private void AddCommonResponseFields(ResponseHeader responseSession)
        {
            throw new NotImplementedException();
        }

        private bool KeepConnection(Session session)
        {
            throw new NotImplementedException();
        }
    }
}
