using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolEngine.Engine.Http1Engine
{
    class FilterFactory
    {
        public IFilter CreateForbidden(string regex)
        {
            return null;
        }
    }
}
