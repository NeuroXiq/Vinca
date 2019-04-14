using ProtocolEngine.Http.Http1.Protocol;
using System;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Http.Http1.Protocol
{

    public struct ContentLengthHf : IHeaderField
    {
        public HFType Type { get { return HFType.ContentLength; } }

        public long ContentLength;

        public ContentLengthHf(long length)
        {
            ContentLength = length;
        }
    }

    public struct ContentTypeHf : IHeaderField
    {
        public HFType Type { get { return HFType.ContentType; } }

        public string ContentType;

        public ContentTypeHf(string contentType)
        {
            ContentType = contentType;
        }
    }

    public struct ServerHf : IHeaderField
    {
        public HFType Type { get { return HFType.Server; } }

        public string Name;

        public ServerHf(string name)
        {
            Name = name;
        }
    }

    public struct DateHf : IHeaderField
    {
        public HFType Type { get { return HFType.Date; } }

        public DateTime Date;

        public DateHf(DateTime dateTime)
        {
            Date = dateTime;
        }
    }

    public struct ContentEncodingHf : IHeaderField
    {
        public HFType Type { get { return  HFType.ContentEncoding;  } }

        public EncodingType EncodingType;

        public ContentEncodingHf(EncodingType codingType)
        {
            EncodingType = codingType;
        }
    }
}
