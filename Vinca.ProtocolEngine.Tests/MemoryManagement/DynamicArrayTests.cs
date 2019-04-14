using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtocolEngine.MemoryManagement;
using System.Linq;

namespace Vinca.ProtocolEngine.Tests.MemoryManagement
{
    [TestClass]
    public class DynamicArrayTests
    {
        bool BufferEquals(byte[] expected, DynamicBuffer actual)
        {
            if (actual.DataLength < expected.Length) return false;

            for (int i = 0; i < expected.Length; i++)
            {
                if (actual[i] != expected[i]) return false;
                         
            }

            return true;
        }

        [TestMethod()]
        public void AddData_DataExceedInitBufSize_ValidResult()
        {
            byte[] asdf = new byte[5] { 1, 2, 3, 4, 5 };

            DynamicBuffer array = new DynamicBuffer(1);

            array.Write(asdf, 0, 5);

            Assert.IsTrue(BufferEquals(asdf, array));
            
        }
    }
}
