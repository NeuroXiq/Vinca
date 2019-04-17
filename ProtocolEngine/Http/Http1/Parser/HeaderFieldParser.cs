using ProtocolEngine.Http.Http1.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Http.Http1.Parser
{
    static class HeaderFieldParser
    {
        static readonly int HashTableRowsCount = TypesCount;
        static readonly int TypesCount = Enum.GetNames(typeof(HFType)).Length;

        static FuncHashTable hashTable;

        static HeaderFieldParser()
        {
            Init();
        }

        static void Init()
        {
            //ParseUndefined -> default parsing function, key value are stored as strings
            hashTable = new FuncHashTable(TypesCount, ParseUndefined);

            //string names in hashTable internally are case-insensitive, HOST==host==Host
            hashTable.Add("Host", ParseHost);
            hashTable.Add("Connection", ParseConnection);
            hashTable.Add("Accept-Encoding", ParseAcceptEncoding);
            hashTable.Add("TE", ParseTE);
            hashTable.Add("Authorization", ParseAuthorization);
        }

        //
        // End initialization methods
        //

        private static int FindColonIndex(byte[] buffer, int offset, int length)
        {
            for (int i = offset; i < offset+ length; i++)
            {
                if (buffer[i] == (byte)':')
                    return i;
            }
            return -1;
        }

        public static IHeaderField Parse(byte[] buffer, int offset, int fieldLength)
        {
            int startIndex = offset;
            int colonIndex = FindColonIndex(buffer, offset, fieldLength); if (colonIndex < 0) throw new Exception("':' delimiter not found");
            int nameLength = colonIndex - startIndex;
            int valueStartIndex = colonIndex + 1;
            int valueLength = offset + fieldLength - valueStartIndex; if (valueLength <= 0) throw new Exception("Empty value component");


            string startToEndString = Encoding.ASCII.GetString(buffer, offset, fieldLength);
            string keyString = Encoding.ASCII.GetString(buffer, offset, colonIndex - offset);
            string valueString = Encoding.ASCII.GetString(buffer, valueStartIndex, valueLength);

            var func = hashTable.GetFuncOrDefault(buffer, offset, nameLength);
            if (func == ParseUndefined)
            {
                return GetUndefinedHeader(buffer, offset, fieldLength);
            }

            return func(buffer, valueStartIndex, valueLength);
        }

        //
        // Parsing methods definitions
        //

        //
        // Auxiliary methods
        //

        static int WhiteSpaceShift(byte[] buffer, int offset)
        {
            int shift = 0;
            while (buffer[offset + shift] == (byte)' ')
            {
                shift++;
            }
            return shift;
        }

        //
        // Undefined header
        //

        private static IHeaderField GetUndefinedHeader(byte[] buffer, int offset, int length)
        {
            string nameValue = Encoding.ASCII.GetString(buffer, offset, length);
            string[] nv = nameValue.Split(':');

            return new UndefinedHf(nv[0], nv[1]);
        }

        static IHeaderField ParseUndefined(byte[] buffer, int valueStartIndex, int valueLength)
        {
            throw new NotImplementedException("__INTERNAL_ERROR::FieldBinaryParser->ParseUndefined()");
        }

        //
        // Host
        //
        static IHeaderField ParseHost(byte[] buffer, int valueOffset, int valueLength)
        {
            string hostString = Encoding.ASCII.GetString(buffer, valueOffset, valueLength);
            hostString = hostString.Trim();

            return new HostHf(hostString);
        }
        
        //
        // TE 
        //
        private static IHeaderField ParseTE(byte[] buffer, int valueOffset, int valueLength)
        {
            string teString = Encoding.ASCII.GetString(buffer, valueOffset, valueLength).Trim();
            string[] values = teString.Split(',');


            //TEHf tehf = new TEHf(resultBitfield);
            //return tehf;
            return null;
        }

        //
        // Connection
        //
        private static IHeaderField ParseConnection(byte[] buffer, int valueOffset, int valueLength)
        {
            string valueString = Encoding.ASCII.GetString(buffer, valueOffset, valueLength).Trim().ToLower();
            ConnectionType cType;

            if (valueString == "keep-alive")
                cType = ConnectionType.KeepAlive;
            else if (valueString == "close")
                cType = ConnectionType.Close;
            else if (valueString == "upgrade")
                cType = ConnectionType.Upgrade;
            else
            {
                string message = string.Format("Unrecognized value format in 'Connection' field ( ' {0} ' )", valueString);
                throw new HeaderFieldParserException(message);
            }

            return new ConnectionHf(cType);
        }

        //
        // Accept-Encoding
        //
        private static IHeaderField ParseAcceptEncoding(byte[] buffer, int valueOffset, int valueLength)
        {
            //length of white spaces from 'valueOffset'
            int whiteSpaceShift = WhiteSpaceShift(buffer, valueOffset);

            string valueString = Encoding.ASCII.GetString(buffer, valueOffset + whiteSpaceShift, valueLength - whiteSpaceShift);
            string[] codingTypesComponents = valueString.Split(',');

            //list holds encodings associated with 'qvalue' ratio (if present in request header)
            List<AcceptEncodingHf.QValueType> codingWithQValues = new List<AcceptEncodingHf.QValueType>();

            EncodingType acceptableEncodingsFlag = 0;

            for (int i = 0; i < codingTypesComponents.Length; i++)
            {
                string qvalueString;
                string codingString = GetCodingComponent(codingTypesComponents[i], out qvalueString);

                EncodingType parsedEType;
                if (!Enum.TryParse(codingString, true, out parsedEType)) throw new HeaderFieldParserException("Unrecognized coding name: " + codingString);

                // this coding fields contains 'qvalue' ? 
                // e.g. "deflate; q=0.7"
                if (!string.IsNullOrEmpty(qvalueString))
                {
                    double qvalueDouble;
                    if (!double.TryParse(qvalueString, out qvalueDouble)) throw new HeaderFieldParserException("Invalud qvalue component: " + qvalueString);
                    if (qvalueDouble < 0.001 || qvalueDouble > 1) throw new HeaderFieldParserException("Invalid value of qvalue in: " + codingTypesComponents[i]);
                    AcceptEncodingHf.QValueType qValue = new AcceptEncodingHf.QValueType(parsedEType, qvalueDouble);
                    codingWithQValues.Add(qValue);

                    // if qvalue > 0 client can accept this encoding.
                    // Add this concrete encoding type to enable encodings bit field
                    if (qvalueDouble > 0)
                    {
                        acceptableEncodingsFlag |= parsedEType;
                    }
                    
                } 
                else 
                {
                    // if qvalue is not present, client can accept this type of encoding.
                    // just set new flag

                    acceptableEncodingsFlag |= parsedEType; 
                }
                
            }

            //contains any qvalues ?
            if (codingWithQValues.Count > 0)
            {
                return new AcceptEncodingHf(acceptableEncodingsFlag, codingWithQValues.ToArray());
            }
            else
            {
                return new AcceptEncodingHf(acceptableEncodingsFlag);
            }
        }

        private static string GetCodingComponent(string encodingComponent, out string qvalue)
        {
            string[] encodingData = encodingComponent.Split(';');
            if (encodingData.Length > 2 || encodingData.Length < 1) throw new HeaderFieldParserException("Invalid syntax of encoding component: " + encodingData);

            string encodingString = null;
            string qvalueString = null;

            //current implementation replace this 2 types of string representation to other enum names 
            if (encodingData[0].Trim() == "*")
                encodingData[0] = "Any";
            else if (encodingData[0].Trim().ToLower() == "pack200-gzip") encodingData[0] = "pack200_gzip";

            encodingString = encodingData[0].Trim();

            if (encodingData.Length == 2)
            {
                qvalueString = encodingData[1];
            }

            qvalue = qvalueString;
            return encodingString;
        }

        //
        // Authorization
        //

        private static IHeaderField ParseAuthorization(byte[] buffer, int valueOffset, int valueLength)
        {
            string value = Encoding.ASCII.GetString(buffer, valueOffset, valueLength);
            value = value.Trim();
            string[] valueData = value.Split(' ');

            string base64Credentials = valueData[1];
            byte[] credentialsBytes  = Convert.FromBase64String(base64Credentials);
            string credentials = Encoding.ASCII.GetString(credentialsBytes);

            string[] credData = credentials.Split(':');
            if (credData.Length != 2)
                throw new HeaderFieldParserException("Invalid data of authorization value");

            string userName = credData[0];
            string password = credData[1];

            AuthorizationHf authHf = new AuthorizationHf(userName, password);

            return authHf;
        }
    }
}
