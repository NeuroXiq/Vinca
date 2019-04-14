using ProtocolEngine.Http1.Protocol;
using ProtocolEngine.MemoryManagement;
using System;
using System.IO;

namespace ProtocolEngine.Engine.Http1Engine
{
    public class MessageStreamReader
    {
        int maxHeaderSize;


        public MessageStreamReader(int maxHeaderSize)
        {
            this.maxHeaderSize = maxHeaderSize;
        }

        /// <summary>
        /// Find '\r\n\r\n' in <paramref name="buffer"/> or read bytes from stream and append to <paramref name="buffer"/> until first end of header occur.
        /// </summary>
        ///<returns>Length of http/1.1 header.</returns>
        ///<exception><see cref="MessageStreamReaderException"/>if readed 0 bytes from stream or if header not found after reach maxHeaderSize</exception>
        public int ReadNextHeader(Stream readStream, DynamicBuffer buffer)
        {
            int readChunkSize = 1024;
            int allReadedBytes = 0;
            int readedBytes = 0;

            while (true)
            {
                int startSearchIndex = allReadedBytes - readedBytes - 3;
                if (startSearchIndex < 0) startSearchIndex = 0;

                //allReadedBytes += readedBytes;


                int headerIndex;// = FindEndOfHeader(buffer, 0);
                headerIndex = FindEndOfHeader(buffer, startSearchIndex);

                if (headerIndex > 0)
                {
                    if (headerIndex + 1 > maxHeaderSize) throw new MessageStreamReaderException("Header size exceed maximum number of bytes");
                    return headerIndex + 4;
                }
                if (allReadedBytes > maxHeaderSize) throw new MessageStreamReaderException("Header size exceed maximum number of bytes");

                readedBytes = buffer.Write(readStream, readChunkSize);
                allReadedBytes += readedBytes;

                if (readedBytes == 0)
                    throw new MessageStreamReaderException("Cannot read from stream. Stream returns 0 bytes after read call");
            }
        }

        private int FindEndOfHeader(DynamicBuffer buffer, int startIndex)
        {
            if (buffer.DataLength < 4) return -1;

            byte[] eohBytes = Http1MessageFormatConsts.EndOfHeaderBytes;
           
            for (int i = 0; i < buffer.DataLength - 3; i++)
            {
                if (eohBytes[0] == buffer[i] && eohBytes[0] == buffer[i + 2])
                {
                    if (eohBytes[1] == buffer[i + 1] && eohBytes[1] == buffer[i + 3])
                        return i;
                }
            }

            return -1;
        }
    }
}
