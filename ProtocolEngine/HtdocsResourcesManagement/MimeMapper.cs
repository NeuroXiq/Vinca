using System.Collections.Generic;
using System.IO;

namespace ProtocolEngine.HtdocsResourcesManagement
{
    class MimeMapper
    {
        static Dictionary<string, string> extensionToMimeMap;
        PathResolver pathResolver;

        const string UnknowMimeString = "application/octet-stream";

        public MimeMapper(Dictionary<string, string> extToMimeMap, PathResolver pathResolver)
        {
            extensionToMimeMap = extToMimeMap;
            this.pathResolver = pathResolver;
        }

        public string GetMime(string fileName)
        {

            fileName = pathResolver.ToAbsolute(fileName);
            if (fileName.EndsWith("img_1.jpg"))
            {
                string a = "";
            }
            int extensionStart = fileName.LastIndexOf('.') + 1;

            if (extensionStart < 1 || extensionStart >= fileName.Length)
            {
                return UnknowMimeString;
            }
            
            string extension = fileName.Substring(extensionStart, fileName.Length - extensionStart);

            string mappedMime;

            if (extensionToMimeMap.TryGetValue(extension, out mappedMime))
            {
                return mappedMime;
            }

            return UnknowMimeString;   
        }

        public static MimeMapper FromApacheFile(string path, PathResolver pathResolver)
        {
            Dictionary<string, string> extToMime = ParseApacheFile(path);

            return new MimeMapper(extToMime, pathResolver);
        }

        private static Dictionary<string, string> ParseApacheFile(string path)
        {
            // Apache format is simple and parsing is straightforward
            // '#' means Comment line and ignoring it
            // some/mime/type {several tabs } ext1 {white space} ext2 {white space} ext3 {.....}
            //e.g.:
            // # application/alto-error+json
            // image/jpeg					jpeg jpg jpe
            // model/vnd.gtw					gtw
            // etc....

            Dictionary<string, string> extToMimeTypeDictionary = new Dictionary<string, string>();

            string mimeTypesFilePath = path;

            using (FileStream fileStream = new FileStream(mimeTypesFilePath, FileMode.Open))
            {
                StreamReader reader = new StreamReader(fileStream);
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length < 1) continue;
                    if (line[0] == '#') continue;

                    // parse mime-type and all associated extensions
                    string[] extensions;
                    string mimeType;
                    ParseApacheMimeTypeLine(line, out mimeType, out extensions);

                    //insert all extensions to dictionary with associated mime-type
                    foreach (string ext in extensions)
                    {
                        //TODO Apache mime type mapper problem
                        //
                        //
                        // ignoring repeated extensions in current version.
                        // this should be resolved in future.
                        // After debug, apache config file contains repeated 
                        // mapping from extension to mime-type e.g.
                        // mime1 -> ext1
                        // mime2 -> fgh
                        // ... -> ... {later ... }
                        // mime123 -> ext1
                        //
                        // as shown above, 2 mime maps to 1 extension.
                        // this feature should be resolved by adding 'string[]' in dictionary instead of 'string'
                        // as a value of mime type and update this array if key already exist.
                        // But this is not necessary to do it now and all works fine
                        // when this case is ignored.

                        if (!extToMimeTypeDictionary.ContainsKey(ext))
                            extToMimeTypeDictionary.Add(ext, mimeType);
                    }

                } // end of while
            }//end of using


            return extToMimeTypeDictionary;
        }

        private static void ParseApacheMimeTypeLine(string line, out string mimeType, out string[] extensions)
        {
            line = line.Trim();

            string mimeTypeSubstring;
            string fileExtensionsSubstring;

            int lastTabIndex = line.LastIndexOf('\t');
            int firstTabIndex = line.IndexOf('\t');

            mimeTypeSubstring = line.Substring(0, firstTabIndex);
            fileExtensionsSubstring = line.Substring(lastTabIndex + 1, line.Length - lastTabIndex - 1);

            mimeType = mimeTypeSubstring;
            extensions = fileExtensionsSubstring.Split(' ');
        }
    }
}
