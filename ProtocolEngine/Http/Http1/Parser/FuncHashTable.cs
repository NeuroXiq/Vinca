using System.Text;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Http.Http1.Parser
{
    ///<summary>Very simple hash table</summary>
    class FuncHashTable
    {
        public delegate IHeaderField FieldParseFunc(byte[] buffer, int valueOffset, int valueLength);

        const int ToLowerDelta = ('a' - 'A');
        const int HashInitValue = 0x12ab34cd;

        class HashRow
        {
            public HashRow Next;
            public string Name;
            public FieldParseFunc ParseFunc;
        }


        HashRow[] HashRows;
        int size;
        private FieldParseFunc defaultFunc;

        public FuncHashTable(int size, FieldParseFunc defaultFunc)
        {
            HashRows = new HashRow[size];
            this.size = size;
            this.defaultFunc = defaultFunc;
        }

        public FieldParseFunc GetFuncOrDefault(byte[] buffer, int offset, int length)
        {
            int index = GetFastHash(buffer, offset, length) % size;

            //string s = Encoding.ASCII.GetString(buffer, offset, length);

            HashRow cur = HashRows[index];
            while (cur != null)
            {
                if (CompareBytes(buffer, offset, length, cur.Name))
                    return cur.ParseFunc;
                else cur = cur.Next;
            }

            return defaultFunc;
        }

        public void Add(string fieldName, FieldParseFunc parseHostField)
        {
            int index = GetFastHash(fieldName) % size;
            string lowerFieldName = fieldName.ToLower();

            if (HashRows[index] == null)
            {
                HashRows[index] = new HashRow()
                {
                    ParseFunc = parseHostField,
                    Name = fieldName,
                    Next = null
                };
            }
            else
            {
                HashRow cur = HashRows[index];
                while (cur.Next != null) cur = cur.Next;

                cur.Next = new HashRow()
                {
                    Name = fieldName,
                    Next = null,
                    ParseFunc = parseHostField
                };
            }

        }

        private static bool CompareBytes(byte[] buffer, int offset, int length, string name)
        {
            if (name.Length != length) return false;

            for (int i = 0; i < length; i++)
            {
                if ((byte)name[i] != buffer[i + offset]) return false;
            }
            return true;
        }

        private static int GetFastHash(string headerName)
        {
            int hash = HashInitValue;

            for (int i = 0; i < headerName.Length; i++)
            {
                if ((byte)headerName[i] >= (byte)'Z') hash = (hash * headerName[i]) + hash;
                else hash = (hash * (headerName[i] + ToLowerDelta)) + hash;
            }

            hash &= 0x7fffffff;
            return hash;
        }

        private static int GetFastHash(byte[] buffer, int offset, int length)
        {
            int hash = HashInitValue;

            for (int i = offset; i < offset + length; i++)
            {
                if (buffer[i] >= (byte)'Z')
                {
                    hash = (hash * buffer[i]) + hash;
                }
                else
                {
                    hash = (hash * (buffer[i] + ToLowerDelta)) + hash;
                }
            }
            hash &= 0x7fffffff;
            return hash;
        }


    }
}
