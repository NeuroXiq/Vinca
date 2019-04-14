using System.Collections.Generic;
using Vinca.ProtocolEngine.Http1.Protocol;

namespace ProtocolEngine.Http.Http1.Protocol
{
    public class HeaderFieldsContainer
    {
        public int Count { get { return  headerFieldsList.Count; } }

        private List<IHeaderField> headerFieldsList;

        public HeaderFieldsContainer(IHeaderField[] headerFields)
        {
            this.headerFieldsList = new List<IHeaderField>(headerFields);
        }

        public HeaderFieldsContainer()
        {
            headerFieldsList = new List<IHeaderField>();
        }

        public IHeaderField this[int i]
        {
            get
            {
                return headerFieldsList[i];
            }
        }

        public void Add(IHeaderField headerField)
        {
            headerFieldsList.Add(headerField);
        }

        public void AddRange(IHeaderField[] fields)
        {
            headerFieldsList.AddRange(fields);
        }

        internal bool TryGetFirst(HFType type, out IHeaderField field)
        {
            foreach (var hf in headerFieldsList)
            {
                if (hf.Type == type)
                {
                    field = hf;
                    return true;
                }
            }

            field = null;
            return false;
        }
    }
}
