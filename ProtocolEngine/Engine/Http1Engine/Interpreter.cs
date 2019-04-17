using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using ProtocolEngine.Engine.Http1Engine.AbstractLayerImplementation;
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

        IFieldInjectFilter[] customInjectFilters;
        IPayloadIgnoreFilter[] customPIgnoreFilters;
        MimeMapper mimeMapper;

        public Interpreter(IHtdocsSystem htdocsSystem, IFieldInjectFilter[] customFI, IPayloadIgnoreFilter[] customPF, MimeMapper mimeMapper)
        {
            this.htdocsSystem = htdocsSystem;
            this.mimeMapper = mimeMapper;
            this.customInjectFilters = customFI;
            this.customPIgnoreFilters = customPF;

        }


        public void Start(Session session)
        {
            do
            {
                session.TakeNextHeader();
                RequestHeader requestHeader = session.CurrentHeader;
                ResponseHeader responseSession = new ResponseHeader();

                bool sendPayload = GoPayloadIgnoreFilters(requestHeader, responseSession);

                if (sendPayload)
                {
                    GoInjectFilters(requestHeader, responseSession);
                }

                InjectCommonFields(requestHeader, responseSession);

                if (sendPayload)
                {
                    responseSession.StatusCode = StatusCode.OK;
                    InjectPayloadFields(requestHeader, responseSession);
                }
                else responseSession.Add(new ContentLengthHf(0));
                SendHeader(session, responseSession);
                if (sendPayload & requestHeader.Method == HttpMethod.GET)
                {
                    SendPayload(session, responseSession);
                }
                
            } while (KeepConnection(session));
        }

        private void InjectPayloadFields(RequestHeader requestHeader, ResponseHeader responseSession)
        {
            string fname = requestHeader.Target.Path;
            string mime = mimeMapper.GetMime(fname);
            long len = htdocsSystem.GetLength(fname, EncodingType.Identity);

            responseSession.Add(new ContentLengthHf(len));
            responseSession.Add(new ContentTypeHf(mime));


        }

        private void InjectCommonFields(RequestHeader requestHeader, ResponseHeader responseSession)
        {
            responseSession.Add(new ServerHf("Vinca/0.8"));
            responseSession.Add(new DateHf(DateTime.Now));
        }

        private void GoInjectFilters(RequestHeader requestHeader, ResponseHeader responseSession)
        {
            if (customInjectFilters != null)
            {
                foreach (IFieldInjectFilter injectFilter in customInjectFilters)
                {
                    if (injectFilter != null)
                    {
                        injectFilter.InjectField(requestHeader, responseSession);
                    }
                }
            }
        }

        private bool GoPayloadIgnoreFilters(RequestHeader requestHeader, ResponseHeader responseHeader)
        {
            if (customPIgnoreFilters != null)
            {
                foreach (IPayloadIgnoreFilter piFilter in customPIgnoreFilters)
                {
                    if (piFilter != null)
                    {
                        if (!piFilter.Check(requestHeader, responseHeader)) return false;
                    }
                }
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
