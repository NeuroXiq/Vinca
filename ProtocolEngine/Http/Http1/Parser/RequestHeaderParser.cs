using System;
using ProtocolEngine.Http.Http1.Protocol;
using Vinca.ProtocolEngine.Http1.Protocol;
using System.Text;
using System.Collections.Generic;

namespace ProtocolEngine.Http.Http1.Parser
{
    public class RequestHeaderParser
    {
        private RequestTargetParser requestTargetParser;

        public RequestHeaderParser()
        {
            requestTargetParser = new RequestTargetParser();
        }

        public RequestHeader Parse(byte[] array, int offset, int length)
        {
            // 1. parse method line
            HttpMethod method;
            HttpVersion version;
            RequestTarget requestTarget;

            int methodLineLen = ParseMethodLine(array, offset, length, out method, out version, out requestTarget);

            int headersStartOffset = IndexOfCrlf(array, offset, length) + 2;

            //2. try parse all headers
            IHeaderField[] headers;

            try
            {
                headers = ParseHeaderFields(array, headersStartOffset, length);
            }
            catch (HeaderFieldParserException e)
            {
                throw new RequestHeaderParserException("HeaderFieldsParser: "+e.Message);
            }
            

            return new RequestHeader(method,requestTarget,version,headers);
        }

        private int ParseMethodLine(byte[] array, int offset, int length, out HttpMethod method, out HttpVersion version, out RequestTarget requestTarget)
        {
            int crlfIndex = IndexOfCrlf(array, offset, length);
            string methodLineString = Encoding.ASCII.GetString(array, offset, crlfIndex - offset);

            string[] methodLineData = methodLineString.Split(' ');

            if (methodLineData.Length != 3 || string.IsNullOrWhiteSpace(methodLineString))
                throw new RequestHeaderParserException("Invalid format of method line");

            method = ParseMethod(methodLineData[0]);
            requestTarget = ParseRequestTarget(methodLineData[1]);
            version = ParseVersion(methodLineData[2]);

            return crlfIndex - offset + 2;
        }

        private IHeaderField[] ParseHeaderFields(byte[] array, int headerStartOffset, int length)
        {
            int fieldStartOffset = headerStartOffset;
            int fieldLength = IndexOfCrlf(array, headerStartOffset, length) - fieldStartOffset;
            List<IHeaderField> headerFields = new List<IHeaderField>();

            while (fieldLength > 0)
            {
                //string fullHeader = Encoding.ASCII.GetString(array, startOffset, endOffset - startOffset);

                IHeaderField field = HeaderFieldParser.Parse(array, fieldStartOffset, fieldLength);
                headerFields.Add(field);

                fieldStartOffset += fieldLength + 2;
                fieldLength = IndexOfCrlf(array, fieldStartOffset, length) - fieldStartOffset;
            }

            return headerFields.ToArray();
        }

        private HttpVersion ParseVersion(string versionString)
        {
            
            if (versionString.Length != 8) throw new RequestHeaderParserException("Invalid format of version component in method line");

            int major = (versionString[5] - '0');
            int minor = (versionString[7] - '0');

            HttpVersion version = new HttpVersion(major, minor);

            return version;
        }

        private RequestTarget ParseRequestTarget(string requestTargetString)
        {
            return requestTargetParser.ParseRequestTargetString(requestTargetString);
        }

        private HttpMethod ParseMethod(string methodString)
        {
            HttpMethod method;
            bool parseResult = Enum.TryParse<HttpMethod>(methodString, out method);

            if (!parseResult) throw new RequestHeaderParserException("Unrecognized method in method line");

            return method;
        }

        ///<summary>First index of \r\n from 'offset'</summary>
        ///<returns>-1 if not found otherwise offset of first byte of the crlf</returns>
        private int IndexOfCrlf(byte[] array, int offset, int length)
        {
            for (int i = offset; i < length - 1; i++)
            {
                if (array[i] == '\r')
                    if (array[i + 1] == '\n')
                        return i;  
            }

            return -1;
        }
    }
}
