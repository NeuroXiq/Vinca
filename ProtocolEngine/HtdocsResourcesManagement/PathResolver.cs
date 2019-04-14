using System.IO;

namespace ProtocolEngine.HtdocsResourcesManagement
{
    class PathResolver 
    {
        string rootDirectory;


        public PathResolver(string rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        public string ToAbsolute(string relativePath)
        {
            string rootDir = rootDirectory;

            if (IsOrigin(relativePath))
            { 
               return Path.Combine(rootDir, "index.html");
            }

            string fullFileName = CombineToAbsolute(rootDir, relativePath);

            return fullFileName;
        }

        private bool IsOrigin(string relativePath)
        {
            return relativePath == "/";
        }

        private string CombineToAbsolute(string root, string relative)
        {
            if (root.EndsWith("\\"))
            {
                root = root.TrimEnd('\\');    
            }
            
            relative = relative.Replace('/', '\\');
            return root + Path.DirectorySeparatorChar + relative;

        }
    }
}
