using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolEngine.Http1.Protocol
{
    public static class Http1MessageFormatConsts
    {
        public const string EndOfLine = "\r\n";
        public const string EndOfHeader = "\r\n\r\n";

        public static readonly byte[] EndOfLineBytes = Encoding.ASCII.GetBytes(EndOfLine);
        public static readonly byte[] EndOfHeaderBytes = Encoding.ASCII.GetBytes(EndOfHeader); 

    }
}
