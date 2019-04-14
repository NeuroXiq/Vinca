using System;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Http.Http1.Protocol
{
    //
    // START COMMONN HEADER FIELDS
    //


    // Special structures
    public struct UndefinedHf : IHeaderField
    {
        public HFType Type { get { return HFType.Undefined; }  }

        public string Name;
        public string Value;

        public UndefinedHf(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public struct ConnectionHf : IHeaderField
    {
        public HFType Type { get { return HFType.Connection; } }

        public ConnectionType ConnectionType;

        public ConnectionHf(ConnectionType type)
        {
            ConnectionType = type;
        }
    }

    //
    // END COMMON STRUCT HEADER FIELDS
    //

    //
    // START REQUEST HEADER FIELDS
    //

    ///<summary>Transfer-Encoding which user agent is willing to accept</summary>
    public struct TEHf : IHeaderField
    {
        public HFType Type { get { return HFType.TE; } }

        ///<summary><see cref="FlagsAttribute"/> enum indicates transfer encodings</summary>
        public EncodingType TransferEncodings;

        //public TEHf(TransferEncodingType encodingTypes)
        //{
        //    TransferEncodings = encodingTypes;
        //}
    }

    public struct HostHf :IHeaderField
    {
        public HFType Type { get { return HFType.Host; } }
        public string Host;

        public HostHf(string host)
        {
            Host = host;
        }
    }

    ///<summary>Indicates payload encoding which client can accept</summary>
    public struct AcceptEncodingHf : IHeaderField
    {
        // associated Qvalues with coding type.
        public struct QValueType
        {
            public EncodingType CodingType;
            public double QValue;

            public QValueType(EncodingType type, double qvalue)
            {
                CodingType = type;
                QValue = qvalue;
            }
        }

        public HFType Type { get { return HFType.AcceptEncoding; } }

        ///<summary>
        /// Flags enum indicates possible coding types present in request header.
        /// This enum contains only encoding with qvalue (if present) greated than 0.
        ///</summary>
        public EncodingType ContentCoding;

        ///<summary>Array contains coding type with associated qvalue (if present).
        ///This field can be null (no qvalues) and can be ignored</summary>
        public QValueType[] QValueCodings;

        public AcceptEncodingHf(EncodingType codingTypes) : this(codingTypes, null)
        {
        
        }

        public AcceptEncodingHf(EncodingType codingTypes, QValueType[] qvalueCodings)
        {
            ContentCoding = codingTypes;
            QValueCodings = qvalueCodings;
        }
    }
}
