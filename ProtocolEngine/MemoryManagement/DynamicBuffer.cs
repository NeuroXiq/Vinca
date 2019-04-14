using System;
using System.IO;

namespace ProtocolEngine.MemoryManagement
{
    // TODO DynamicArray BufferPool problem. 
    // Buffer must be explicitly relased (call Dispose()). 

    ///<summary>Extensible byte array</summary>
    public class DynamicBuffer : IDisposable
    {
        int size;
        int dataLength;

        byte[] buffer;

        public int Length { get { return size; } }
        public int DataLength { get { return dataLength; } }
        public byte[] BufferArray { get { return buffer; } }

        public DynamicBuffer() : this(1024)
        {
            
        }

        public DynamicBuffer(int initSize)
        {
            buffer = BufferPool.Take(initSize) ;
            size = initSize;
        }

        public byte this[int index]
        {
            get { return buffer[index]; }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            ExtendIfNeed(count);

            int j = dataLength;
            for (int i = offset; i < offset + count; i++, j++)
            {
                this.buffer[j] = buffer[i];
            }

            dataLength += count;
        }

        public int Write(Stream stream, int count)
        {
            ExtendIfNeed(count);

            int writed = stream.Read(buffer, dataLength, count);
            dataLength += writed;

            return writed;
        }

        ///<summary>
        /// Removes bytes from <paramref name="startIndex"/> 
        /// and shifts all the remaining bytes to the reclaimed space. 
        /// </summary>
        public void ResetTo(int startIndex, int count)
        {
            if (count == 0 || dataLength == 0) return;
            if (startIndex >= dataLength || startIndex < 0) throw new ArgumentOutOfRangeException("startIndex");
            if (startIndex + count > dataLength) throw new ArgumentOutOfRangeException("count");

            int writeStart = startIndex;
            int copyFrom = startIndex + count;
            int copyCount = dataLength - (startIndex + count);

            if (copyCount > 0)
            {
                Buffer.BlockCopy(
                    buffer,
                    copyFrom,
                    buffer,
                    writeStart,
                    copyCount);    
            }

            dataLength -= count;
        }

        private void ExtendIfNeed(int count)
        {
            int dataLenAfterWrite = count + dataLength;

            //need to extend internal buffer size?
            if (size < dataLenAfterWrite)
            {
                int newSize = size * 2;
                newSize = newSize < dataLenAfterWrite ? dataLenAfterWrite : newSize;
                
                // 1. take bigger buffer from pool
                byte[] newBuff = BufferPool.Take(newSize);

                //2. copy current buffer to bigger one
                Buffer.BlockCopy(buffer, 0, newBuff, 0, dataLength);

                //3. relase previouse buffer to pool
                BufferPool.Free(buffer);

                //4. update private fields
                buffer = newBuff;
                size = newSize;
            }
        }

        public void Dispose()
        {
            // internally this class use buffers from 'BufferPool' class
            // it is not necessary to relase this buffer but it should be done 

            BufferPool.Free(buffer);
        }
    }
}
