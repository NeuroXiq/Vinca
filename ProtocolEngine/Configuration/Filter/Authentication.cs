using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolEngine.Configuration.Filter
{
    [Serializable]
    public class Authentication
    {
        public string FileNameRegex;
        public string UserName;
        public string Password;
    }
}
