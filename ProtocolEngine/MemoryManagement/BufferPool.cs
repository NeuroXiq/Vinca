using System;
using System.Collections.Generic;
using System.Threading;

namespace ProtocolEngine.MemoryManagement
{
    ///<summary>
    /// Provides allocated, reausable byte buffers in concurrent mode (class is thread safe). 
    /// Recommended way to create new byte[] array is to use this class.
    ///</summary>
    public static class BufferPool
    {
        private static BufferInfo[] DefaultBufferInfoInit = new BufferInfo[]
        {
            new BufferInfo(50, 1 * 512),
            new BufferInfo(25, 2 * 512),
            new BufferInfo(15, 4 * 512),
            new BufferInfo(5,  8 * 512),
        };

        public struct BufferInfo
        {
            public int MaxHold;
            public int Size;

            ///<summary>Informations about buffer</summary>
            ///<param name="maxHold">Maximum number of buffers of size '<see cref="size"/>'  which <see cref="BufferPool"/> will hold</param>
            ///<param name="size">Size of buffer in bytes</param>
            public BufferInfo(int maxHold, int size)
            {
                MaxHold = maxHold;
                Size = size;
            }
        }

        struct StackData
        {
            public int MinLength;
            public int MaxOnStack;
            public Stack<byte[]> BuffersStack;

            public StackData(int minLen, int maxOn)
            {
                MinLength = minLen;
                MaxOnStack = maxOn;
                BuffersStack = new Stack<byte[]>();
            }
        }

        private static object synchObject = new object();

        private static StackData[] stacksData;

        static BufferPool()
        {
            Initialize();
        }

        private static void Initialize()
        {
            SetBuffers(DefaultBufferInfoInit);
        }

        public static byte[] Take(int minSize)
        {
            //try to find the smallest buffer satisfying 'minSize' value
            byte[] buffer = null;
            bool continueSearch = true;
            int smallestBufferIndex = -1;

            for (int i = 0; i < stacksData.Length && continueSearch; i++)
            {
                if (minSize <= stacksData[i].MinLength)
                {
                    // alignment to smallest which is greated or equal 'minSize'.
                    if (smallestBufferIndex < 0) smallestBufferIndex = i;
                     
                    lock (synchObject)
                    {
                        if (stacksData[i].BuffersStack.Count > 0)
                        {
                            buffer = stacksData[i].BuffersStack.Pop();
                            continueSearch = false;
                        }
                        else continueSearch = true;
                    }
                }
            }

            //cannot take any buffer from allocated stacks. Need to create new
            if (continueSearch)
            {

                // alignment of buffer length is performed.
                // if smallestBufferIndex is still -1, 'minValue' exceed largest buffer size. 
                // Need to create new buffer with this provided in 'minValue' param.
                if (smallestBufferIndex < 0)
                {
                    buffer = new byte[minSize];
                }
                else
                { 
                    //alignment to the smallest buffer size that is greater or equal to provided 'minSize' value
                    int alignmentSize = stacksData[smallestBufferIndex].MinLength;
                    buffer = new byte[alignmentSize];
                }
            }

            return buffer;
        }

        public static void Free(byte[] buffer)
        {

            int size = buffer.Length;
            int i = 0;
            while (stacksData[i].MinLength < size)
            {
                i++;
                if (i >= stacksData.Length) break;
            }

            if (i >= stacksData.Length)
            {
                // buffer is greater than max size.
                // not sure what to do with this buffer. 
                // current implementation ignore this case and leave this work for GC.

                //nothing
            }
            else
            {
                lock (synchObject)
                {
                    // every stack have own treshold.
                    int nowOnStack = stacksData[i].BuffersStack.Count;
                    int stackCanHold = stacksData[i].MaxOnStack;

                    if (stackCanHold > nowOnStack)
                    {
                        // ok 
                        stacksData[i].BuffersStack.Push(buffer);
                    }
                    else
                    {
                        // nothing, ignore this buffer and GC do work
                    }
                }
            }
        }

        /// <summary>
        /// Allocate all buffers to maximum treshold.
        /// This method should be called only once.
        /// </summary>
        /// <example>
        /// <code>
        /// BufferInfo[] bufferInfo = new BufferInfo[] { ... Informations about buffers to set ... }
        /// BufferPool.SetBuffer(buffers);
        /// BufferPool.FillToMax();
        /// </code>
        /// </example>
        public static void FillToMax()
        {
            lock (synchObject)
            {
                foreach (StackData sData in stacksData)
                {
                    //current stack contains less buffers as can hold?
                    if (sData.BuffersStack.Count < sData.MaxOnStack)
                    {
                        //fill to max treshold
                        int countToFill = sData.MaxOnStack - sData.BuffersStack.Count;
                        int bufSize = sData.MinLength;
                        for (int i = 0; i < countToFill; i++)
                        {
                            sData.BuffersStack.Push(new byte[bufSize]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set buffers treshold and size.
        /// <example>
        /// <code>
        /// BufferInfo[] bufferInfo = new BufferInfo[] { ... Informations about buffers to set ... }
        /// BufferPool.SetBuffer(buffers);
        /// BufferPool.FillToMax();
        /// </code>
        /// </example>
        /// </summary>
        public static void SetBuffers(BufferInfo[] buffersData)
        {
            lock (synchObject)
            {
                stacksData = new StackData[buffersData.Length];
                for (int i = 0; i < stacksData.Length; i++)
                {
                    stacksData[i] = new StackData(buffersData[i].Size, buffersData[i].MaxHold);
                }

                Comparison<StackData> stackDataComparer = delegate (StackData s1, StackData s2) 
                {
                    return -(s1.MaxOnStack - s2.MaxOnStack);
                };

                //need to sort in ascending order
                Array.Sort(stacksData, stackDataComparer);
            }
        }
    }
}
