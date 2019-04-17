using ProtocolEngine.Http.Http1.Protocol;
using System;
using System.Globalization;
using System.Text;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Http.Http1.Formatters
{
    public class HfBinaryFormatter
    {
        static readonly int HFTypesCount = Enum.GetNames(typeof(HFType)).Length;

        delegate int FormatValueFunc(byte[] buffer, int offset, IHeaderField headerField);
        delegate int LengthFunc(IHeaderField field);

        static FormatValueFunc[] FormatFuncs;
        static LengthFunc[] LenFuncs;
        static byte[][] BinaryHeaderName;

        static HfBinaryFormatter()
        {
            InitFormatter();
        }

        private static void InitFormatter()
        {
            FormatFuncs = new FormatValueFunc[HFTypesCount];
            LenFuncs = new LengthFunc[HFTypesCount];
            BinaryHeaderName = new byte[HFTypesCount][];

            InsertFormatterInfo(HFType.ContentLength, "Content-Length:", FormatContentLength, FormattedContentLengthLength);
            InsertFormatterInfo(HFType.Server, "Server:", FormatServer, FormattedServerValueLength);
            InsertFormatterInfo(HFType.Date, "Date:", FormatDate, FormattedDateLength);
            InsertFormatterInfo(HFType.ContentType, "Content-Type:", FormatContentType, FormattedContentTypeLength);
            InsertFormatterInfo(HFType.ContentEncoding, "Content-Encoding:", FormatContentEncoding, FormattedContentEncodingLength);
            InsertFormatterInfo(HFType.Location, "Location:", FormatLocation, FormattedLocationLength);
            InsertFormatterInfo(HFType.WWWAuthenticate, "WWW-Authenticate:", FormatWWWAuthenticate, FormattedWWWAuthenticateLength);
        }

        

        private static void InsertFormatterInfo(HFType type, string hfName, FormatValueFunc fvf, LengthFunc lf)
        {
            int index = (int)type;

            FormatFuncs[(index)] = fvf;
            LenFuncs[index] = lf;
            BinaryHeaderName[index] = Encoding.ASCII.GetBytes(hfName);
        }

        public int GetFormattedLength(IHeaderField field)
        {
            int nLen = -1;

            //Only one special case
            if (field.Type == HFType.Undefined)
            {
                
                 nLen = ((UndefinedHf)field).Name.Length; //name
                 nLen += ((UndefinedHf)field).Value.Length; // value
                 nLen += 1; // ':'
            }
            else
            {
                int index = (int)field.Type;
                var func = LenFuncs[index];
                nLen = BinaryHeaderName[index].Length;
                nLen += func(field);
            }

            return  nLen + 2;
        }

        public int FormatBinaryLine(byte[] buffer, int offset, IHeaderField field)
        {
            //Only one special case
            if (field.Type == HFType.Undefined) return FormatUndefined(field,buffer,offset);

            int index = (int)field.Type;
            var func = FormatFuncs[index];
            if (func == null) throw new NotSupportedException("HfBinaryFormatter do not support " + field.Type.ToString());

            //1. copy header name bytes
            int bhNameLen = BinaryHeaderName[index].Length;
            for (int i = 0; i < bhNameLen; i++)
            {
                buffer[offset + i] = BinaryHeaderName[index][i];
            }

            //2. write value
            int valueLen = func(buffer, offset + bhNameLen, field);
            int crlfIndex = offset + valueLen + bhNameLen;

            //3. write crlf
            buffer[crlfIndex + 0] = (byte)'\r';
            buffer[crlfIndex + 1] = (byte)'\n';

            // ('2' == crlf length)
            return 2 + bhNameLen + valueLen;
        }

        //
        // Special undefined header
        //
        private int FormatUndefined(IHeaderField field, byte[] buffer, int offset)
        {
            string nameStr = ((UndefinedHf)(field)).Name;
            string valueStr = ((UndefinedHf)(field)).Value;
            
            Encoding.ASCII.GetBytes(nameStr, 0, nameStr.Length, buffer, offset);
            buffer[offset + nameStr.Length] = (byte)':';
            Encoding.ASCII.GetBytes(valueStr, 0, valueStr.Length, buffer, offset + nameStr.Length + 1);
            int endIndex = offset+ 1 + nameStr.Length + valueStr.Length;

            buffer[endIndex + 0] = (byte)'\r';
            buffer[endIndex + 1] = (byte)'\n';

            return valueStr.Length + nameStr.Length + 1 + 2;
        }

        //
        // value formatter definitions
        //

        //
        // Content-Length
        //

        private static int FormatContentLength(byte[] buffer, int offset, IHeaderField hf)
        {
            ContentLengthHf cl = (ContentLengthHf)hf;
            int len = cl.ContentLength > 0 ? (int)Math.Log10(cl.ContentLength) : 0;
            long curV = cl.ContentLength;

            for (int i = offset + len; i >= offset; i--)
            {
                buffer[i] = (byte)((curV % 10) + (byte)'0');
                curV /= 10;
            }

            return len + 1;
        }

        private static int FormattedContentLengthLength(IHeaderField field)
        {
            long length = ((ContentLengthHf)field).ContentLength;
            if (length > 0)
            {
                int valueLen = (int)Math.Log(length) + 1;
                return valueLen;
            }
            else return 1;                    
        }

        //
        // Server 
        //

        private static int FormatServer(byte[] buffer, int offset, IHeaderField headerField)
        {
            string name = ((ServerHf)headerField).Name;
            for (int i = 0; i < name.Length; i++)
            {
                buffer[i + offset] = (byte)name[i];
            }

            return name.Length;
        }

        private static int FormattedServerValueLength(IHeaderField field)
        {
            return ((ServerHf)field).Name.Length;
        }

        //
        // Date
        //

        private static int FormattedDateLength(IHeaderField field)
        {
            var curDate = ((DateHf)field).Date.ToUniversalTime();
            string dateString = curDate.ToString("ddd, d MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            string fullDateString = string.Format("{0} {1}", dateString, "GMT");

            return fullDateString.Length;
        }

        private static int FormatDate(byte[] buffer, int offset, IHeaderField headerField)
        {
            DateHf date = (DateHf)headerField;
            DateTime curDate = date.Date.ToUniversalTime();
            string dateString = curDate.ToString("ddd, d MMM yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            string fullDateString = string.Format("{0} {1}", dateString, "GMT");
            

            for (int i = 0; i < fullDateString.Length; i++)
            {
                buffer[i + offset] = (byte)fullDateString[i];
            }

            return fullDateString.Length;
        }

        //
        // Content-Type 
        //
        private static int FormattedContentTypeLength(IHeaderField field)
        {
            ContentTypeHf contentType = (ContentTypeHf)field;

            return contentType.ContentType.Length;
        }

        private static int FormatContentType(byte[] buffer, int offset, IHeaderField headerField)
        {
            ContentTypeHf contentType = (ContentTypeHf)headerField;

            for (int i = 0; i < contentType.ContentType.Length; i++)
            {
                buffer[i + offset] = (byte)contentType.ContentType[i];
            }

            return contentType.ContentType.Length;
        }

        //
        // Content-Encoding
        //

        private static int FormattedContentEncodingLength(IHeaderField field)
        {
            ContentEncodingHf ce = (ContentEncodingHf)field;

            ThrowIfInvaludEncodingType(ce.EncodingType);

            return Enum.GetName(typeof(EncodingType), ce.EncodingType).Length; 
        }

        private static int FormatContentEncoding(byte[] buffer, int offset, IHeaderField headerField)
        {
            ContentEncodingHf ce = (ContentEncodingHf)headerField;

            // this should not to be here
            ThrowIfInvaludEncodingType(ce.EncodingType);

            //get string from enum
            string encodingStringValue =  " " + Enum.GetName(typeof(EncodingType), ce.EncodingType).ToLower();

            //inset bytes 
            for (int i = 0; i < encodingStringValue.Length; i++)
            {
                buffer[i + offset] = (byte)encodingStringValue[i];
            }

            //return writted bytes
            return encodingStringValue.Length;
        }

        //
        // Encoding type enum is '[Flag]', can hold several values but to formatter must be present only 1 bit set.
        // 
        private static void ThrowIfInvaludEncodingType(EncodingType type)
        {
            int errorCheck = (int)type;
            if (errorCheck == 0)
                throw new Exception("HfBinaryFormatter.FormattedContentEncodingLength: Trying to format invalid ('0') value");
            while (true)
            {
                if (errorCheck == 1) break;

                if (errorCheck % 2 == 1)
                    if (errorCheck > 1)
                        throw new Exception("HfBinaryFormatter.FormattedContentEncodingLength: Encoding type contains more than 1 set bit");
                errorCheck >>= 1;
            }
        }


        private static int FormattedLocationLength(IHeaderField field)
        {
            LocationHf location = (LocationHf)field;
            return location.Location.Length;
        }

        private static int FormatLocation(byte[] buffer, int offset, IHeaderField headerField)
        {
            LocationHf location = (LocationHf)headerField;

            return Encoding.ASCII.GetBytes(location.Location, 0, location.Location.Length, buffer, offset);
        }

        private static int FormattedWWWAuthenticateLength(IHeaderField field)
        {
            WWWAuthenticateHf authHf = (WWWAuthenticateHf)field;
            int len = authHf.Realm.Length + authHf.Charset.Length + 26;

            return len;
        }

        private static int FormatWWWAuthenticate(byte[] buffer, int offset, IHeaderField field)
        {
            WWWAuthenticateHf authHf = (WWWAuthenticateHf)field;
            string result = string.Format("Basic realm=\"{0}\", charset=\"{1}\"", authHf.Realm, authHf.Charset);
            return Encoding.ASCII.GetBytes(result, 0, result.Length, buffer, offset);
        }
    }
}
