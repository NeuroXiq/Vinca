using ProtocolEngine.Http.Http1.Protocol;
using System;

namespace ProtocolEngine.Http.Http1.Parser
{
    //TODO Add custom exceptions instead of 'Exception'

    ///<summary>Provides methods used to parse request targer from string (HTTP/1.1 specification)</summary>
    public class RequestTargetParser
    {

        public RequestTarget ParseAbsoluteUri(string absoluteUri)
        {
            //2 types of uri
            // (schema  {:} path[?query])
            // ?????????{:}??????????????
            // or 
            // (schema  {://}authority/path[?query])
            // ?????????{://}??????????????????
            //

            //basic substring components or uri
            string pathQuerySubstring = string.Empty;
            string authoritySubstring = string.Empty;

            //specific components of uri
            string scheme;
            string userInfo;
            string host;
            int port;
            string path;
            string query;

            // parsing

            if (absoluteUri.Length < 3) throw new Exception("Uri must contain at least 3 chars");

            string[] uriBaseComponents = absoluteUri.Split(new string[] { "://" }, StringSplitOptions.None);
            if (uriBaseComponents.Length > 2) throw new Exception("Uri cannot contain more than 1 '://' separator");



            if (uriBaseComponents.Length == 2)
            {
                //second uri type

                //schemem must contains at least 1 char 
                if (uriBaseComponents[0].Length == 0) throw new Exception("Uri schema component cannot be empty");
                scheme = uriBaseComponents[0];

                int pathStartIndex = uriBaseComponents[1].IndexOf('/');

                //path can be empty
                
                if (pathStartIndex > -1)
                {
                    //path present
                    //authority substring to first '/'
                    //path+query substring from '/' to end

                    authoritySubstring =
                        uriBaseComponents[1].Substring(0, pathStartIndex);

                    pathQuerySubstring =
                        uriBaseComponents[1].Substring(pathStartIndex, uriBaseComponents[1].Length - pathStartIndex);
                }
                else
                {
                    //path empty
                    //authority string to end
                    //
                    authoritySubstring = uriBaseComponents[1].Substring(0, uriBaseComponents[1].Length);
                    pathQuerySubstring = string.Empty;
                }
            }
            else // must be first uri type
            {
                // uribasecompo[0]     uribasecomp[1]
                //{ schema       } :  { optional path + optional query}  
                //
                // absent authority component 

                authoritySubstring = string.Empty;

                uriBaseComponents = absoluteUri.Split(':');
                if (uriBaseComponents.Length != 2) throw new Exception("Uri with absent authority component contains more than 1 ':' separator");

                //schema must not be empty
                if (uriBaseComponents[0].Length == 0) throw new Exception("schema component cannot be empty");

                // after split, first element of array contains schema string
                scheme = uriBaseComponents[0];

                //path component is empty ?
                if (uriBaseComponents[1].Length == 0)
                {
                    pathQuerySubstring = string.Empty;
                }
                else pathQuerySubstring = uriBaseComponents[1]; // if not, prepare for future parsing
            }

            //prepared 2 types of substrings:
            // 1. authority substring
            // 2. pathQuery substring
            // now they can be safely parsed


            ParsePathQueryString(pathQuerySubstring, out path, out query);
            ParseAuthorityString(authoritySubstring, out userInfo, out host, out port);

            //build result

            RequestTarget target = new RequestTarget();

            target.Form = RequestTarget.RequestTargetForm.Absolute;
            target.Host = host;
            target.Path = path;
            target.Port = port;
            target.Query = query;
            target.Scheme = scheme;
            target.UserInfo = userInfo;

            return target;
        }

        private void ParseAuthorityString(string authoritySubstring, out string userInfo, out string host, out int port)
        {
            if (string.IsNullOrEmpty(authoritySubstring))
            {
                userInfo = string.Empty;
                host = string.Empty;
                port = -1;
            }

            //get user info substring

            int userInfoEnd = authoritySubstring.IndexOf('@');
            string userInfoString = string.Empty;

            if (userInfoEnd > -1) userInfoString = authoritySubstring.Substring(0, userInfoEnd);
            else userInfoString = string.Empty;

            // get host substring

            string hostSubstring = authoritySubstring.Substring(userInfoEnd + 1, authoritySubstring.Length - userInfoEnd - 1);

            // port is present in host substring? 

            string[] hostData = hostSubstring.Split(':');
            string hostString = string.Empty;
            int portInt = -1;

            if (hostData.Length == 2)
            {
                hostString = hostData[0];
                if (!int.TryParse(hostData[1], out portInt)) throw new Exception("Cannot parse port number in authority component");
            }
            else hostString = hostData[0];

            //assign 'out' parameter
            userInfo = userInfoString;
            host = hostString;
            port = portInt;
        }

        private void ParsePathQueryString(string pathQueryString, out string path, out string query)
        {
            path = string.Empty;
            query = string.Empty;

            if (string.IsNullOrEmpty(pathQueryString))
            {
                //its ok, path and query can be empty
                return;    
            }

            string[] pqData = pathQueryString.Split('?');

            if (pqData.Length > 2) throw new Exception("Found more than 1 path/query delimiter '?' in uri");

            // query is present ?
            if (pqData.Length == 2)
            {
                path = pqData[0];
                query = pqData[1];
            }
            else path = pqData[0];


        }

        public RequestTarget ParseRequestTargetString(string requestTarget)
        {
            if (requestTarget.Length < 1)
                throw new Exception("requestTarget must contain at least 1 char");

            //(rfc 7230 -> 5.3 (Request target))
            // Request target can be in 4 formats:  
            //  1. asteriks form (only '*')
            //  2. origin form ('/')
            //  3. authority form (start with userinfo@www. (...)  or  www. (...))
            //  4. absolute form (start with 'http://' or 'https://')


            if (requestTarget.Length == 1 && string.Compare(requestTarget, "*", StringComparison.Ordinal) == 0)
            {
                return GetRequestTargetArteriskForm();
            }
            else if (requestTarget.StartsWith("/", StringComparison.Ordinal))
            {
                return ParseRequestTargetOriginForm(requestTarget);
            }
            else if (requestTarget.StartsWith("http", StringComparison.Ordinal))
            {
                return ParseRequestTargetAbsoluteForm(requestTarget);
            }
            else
            {
                return ParseRequesTargetAuthorityForm(requestTarget);
            }

        }

        public RequestTarget ParseRequestTargetAbsoluteForm(string absoluteForm)
        {
            RequestTarget result;
            try
            {
                result = ParseAbsoluteUri(absoluteForm);
            }
            catch (Exception e)
            {

                throw;
            }

            return result;
        }

        public RequestTarget ParseRequesTargetAuthorityForm(string requestTarget)
        {
            // Authority form: '[]' -> optional
            // [userinfo@]host[:port]
            // but in rfc 7230 5.3.3 says to exclude user info.
            // there are only 2 options:
            //   host
            //   host:port

            string[] requestData = requestTarget.Split(':');
            if (requestData.Length > 2) throw new Exception("Invalid authority form of request target: Multiple ':' separators.");

            int port = -1;
            if (requestData.Length == 2)
            {
                bool portParsed = int.TryParse(requestData[1], out port);

                if (!portParsed || port < 0)
                    throw new Exception("Invalid port in authority form of request target");
            }

            RequestTarget target = new RequestTarget();

            target.Form = RequestTarget.RequestTargetForm.Authority;
            target.Host = requestData[0];
            target.Port = -1;
            target.Path =
                target.Query =
                target.Scheme =
                target.UserInfo =
                string.Empty;

            return target;
        }

        public RequestTarget ParseRequestTargetOriginForm(string requestTarget)
        {
            // origin form:
            // path[?optional_query_string]
            RequestTarget target = new RequestTarget();

            string[] targetData = requestTarget.Split('?'); // if array contain 1 element, no query, if 2 path+query if 3 or more fail

            if (targetData.Length > 2)
                throw new Exception("path component contains multiple '?' chars (query separator)");

            target.Form = RequestTarget.RequestTargetForm.Origin;
            target.Host = string.Empty;
            target.Scheme = string.Empty;
            target.UserInfo = string.Empty;
            target.Path = targetData[0];
            target.Port = -1;
            target.Query = targetData.Length == 2 ? targetData[1] : string.Empty;

            return target;
        }

        //Instead of parsing, this method 'Gets' because asterisk form is constant.
        public RequestTarget GetRequestTargetArteriskForm()
        {
            RequestTarget target = new RequestTarget();

            target.Form = RequestTarget.RequestTargetForm.Asterisk;
            target.Host =
                target.Path =
                target.Query =
                target.Scheme =
                target.UserInfo =
                string.Empty;

            target.Port = -1;

            return target;
        }
    }
}
