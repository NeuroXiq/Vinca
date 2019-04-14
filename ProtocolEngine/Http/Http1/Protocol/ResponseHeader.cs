using System.Collections.Generic;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Http.Http1.Protocol
{
    public class ResponseHeader
    {
        public int FieldsCount { get { return headerFields.Count; } }
        public StatusCode StatusCode;
        List<IHeaderField> headerFields;

        public ResponseHeader()
        {
            headerFields = new List<IHeaderField>();
        }

        public void Add(params IHeaderField[] headers)
        {
            headerFields.AddRange(headers);
        }
        public IHeaderField GetField(int index)
        {
            return headerFields[index];
        }
    }
}
