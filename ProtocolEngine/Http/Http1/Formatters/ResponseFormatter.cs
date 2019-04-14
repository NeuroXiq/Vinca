using ProtocolEngine.Http.Http1.Protocol;
using System;
using System.Text;

namespace ProtocolEngine.Http.Http1.Formatters
{
    public class ResponseFormatter
    {
        static HfBinaryFormatter hfBinaryFormatter = new HfBinaryFormatter();

        public int FormattedLength(ResponseHeader header)
        {
            // 'XXX 
            int len = "HTTP/1.1 200\r\n".Length;
            for (int i = 0; i < header.FieldsCount; i++)
            {
                len += hfBinaryFormatter.GetFormattedLength(header.GetField(i));
            }

            return len + 2;
        }

        public int GetBytes(byte[] buffer, int offset, ResponseHeader header)
        {
            int writedCount = 0;

            string statusLine = string.Format("HTTP/1.1 {0}\r\n",(int)header.StatusCode);
            writedCount += Encoding.ASCII.GetBytes(statusLine, 0, statusLine.Length, buffer, offset);

            for (int i = 0; i < header.FieldsCount; i++)
            {
                
                writedCount += hfBinaryFormatter.FormatBinaryLine(buffer, writedCount + offset, header.GetField(i));
            }
            buffer[writedCount + 0] = (byte)'\r';
            buffer[writedCount + 1] = (byte)'\n';
            return writedCount + 2;
        }
    }
}
