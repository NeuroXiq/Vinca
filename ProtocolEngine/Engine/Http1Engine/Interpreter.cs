using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using ProtocolEngine.HtdocsResourcesManagement;
using ProtocolEngine.Http.Http1.Formatters;
using ProtocolEngine.Http.Http1.Protocol;
using ProtocolEngine.MemoryManagement;
using System;
using System.IO;
using System.Text;
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
            this.htdocsSystem = htdocsSystem;
            this.customFilters = customFilters;
            this.mimeMapper = mimeMapper;
        }


        public void Start(Session session)
        {
            do
            {
                session.TakeNextHeader();
                RequestHeader requestHeader = session.CurrentHeader;
                ResponseHeader responseSession = new ResponseHeader();

                bool sendPayload = Filter(requestHeader, responseSession);

                if (sendPayload)
                    AddPayloadFields(requestHeader, responseSession);


                if (sendPayload)
                    responseSession.StatusCode = StatusCode.OK;

                AddCommonResponseFields(responseSession);

                string fn = session.CurrentHeader.Target.Path;
                if (fn.EndsWith("jpg"))
                {
                    string asd = "asdf";
                }

                SendHeader(session, responseSession);

                

                if (sendPayload && requestHeader.Method == HttpMethod.GET)
                    SendPayload(session, responseSession);

            } while (KeepConnection(session));
        }

        private void AddPayloadFields(RequestHeader requestHeader, ResponseHeader responseSession)
        {
            string fileName = requestHeader.Target.Path;

            string mime = mimeMapper.GetMime(fileName);
            long len = htdocsSystem.GetLength(fileName, EncodingType.Identity);
            //string etag = htdocsSystem.GetETag(fileName, EncodingType.Identity);
            


            responseSession.Add(new ContentTypeHf(mime));
            //responseSession.Add(new UndefinedHf("Accept-Range", "bytes"));
            responseSession.Add(new ContentEncodingHf(EncodingType.Identity));
            //responseSession.Add(new UndefinedHf("ETag", etag));
            responseSession.Add(new ContentLengthHf(len));

        }

        private bool Filter(RequestHeader requestHeader, ResponseHeader responseSession)
        {
            if (htdocsSystem.FileExists(requestHeader.Target.Path))
            {
                return true;
            }
            else
            {
                responseSession.StatusCode = StatusCode.NotFound;
                responseSession.Add(new ContentLengthHf(0));
                return false;
            }





            foreach (IFilter filter in customFilters)
            {
                if (!filter.Check(requestHeader, responseSession))
                    return false;
            }

            foreach (IFilter filter in constantFilters)
            {
                if (!filter.Check(requestHeader, responseSession))
                    return false;
            }

            return true;
        }

        private void SendPayload(Session session, ResponseHeader responseSession)
        {
            string fn = session.CurrentHeader.Target.Path;

            Stream fileStream = htdocsSystem.OpenStream(fn, EncodingType.Identity);
            try
            {
                session.CopyStream(fileStream);
            }
            catch (Exception e)
            {
                htdocsSystem.CloseStream(fileStream);
                throw;
            }
            htdocsSystem.CloseStream(fileStream);
        }

        private void SendHeader(Session session, ResponseHeader responseSession)
        {
            ResponseFormatter formatter = new ResponseFormatter();
            int len = formatter.FormattedLength(responseSession);
            byte[] buf = BufferPool.Take(len);

            len = formatter.GetBytes(buf, 0, responseSession);
            string response = Encoding.ASCII.GetString(buf, 0,len);
            try
            {
                session.Send(buf, 0, len);
            }
            catch (Exception e)
            {
                BufferPool.Free(buf);
                throw;
            }
            BufferPool.Free(buf);
        }

        private void AddCommonResponseFields(ResponseHeader responseSession)
        {
            responseSession.Add(
                new DateHf(DateTime.Now),
                new ServerHf(" Vinca/0.8"));
        }

        private bool KeepConnection(Session session)
        {
            if (session.CurrentHeader.Contains(HFType.Connection))
            {
                ConnectionHf c = session.CurrentHeader.GetSingleField<ConnectionHf>(HFType.Connection);
                return c.ConnectionType == ConnectionType.KeepAlive;
            }

            return true;
        }
    }
}
