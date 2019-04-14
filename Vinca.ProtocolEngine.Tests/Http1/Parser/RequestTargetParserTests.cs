using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtocolEngine.Http.Http1.Parser;
using ProtocolEngine.Http.Http1.Protocol;

namespace Vinca.ProtocolEngine.Tests.Http1.Parser
{
    [TestClass()]
    public class RequestTargetParserTests
    {
        public RequestTarget BuildFullUriAndRequestTarget(string scheme, string userInfo, string host, int port, string path, string query , out string uriString)
        {
            RequestTarget target = new RequestTarget();
            target.Form = RequestTarget.RequestTargetForm.Absolute;

            target.Host = host;
            target.Path = path;
            target.Port = port;
            target.Query = query;
            target.Scheme = scheme;
            target.UserInfo = userInfo;

            string uriFormatterString = string.Format("{0}://{1}@{2}:{3}{4}?{5}", scheme,userInfo,host,port,path,query);

            uriString = uriFormatterString;

            return target;
        }

        public RequestTarget BuildAbsentAuthorityUriAndRequestTarget(string scheme, string path, string query, out string uriString)
        {
            RequestTarget target = new RequestTarget();
            target.Form = RequestTarget.RequestTargetForm.Absolute;

            target.Host = string.Empty;
            target.Path = path;
            target.Port = -1;
            target.Query = query;
            target.Scheme = scheme;
            target.UserInfo = string.Empty ;

            string uriFormatterString = string.Format("{0}:{1}?{2}",scheme,path,query);

            uriString = uriFormatterString;

            return target;
        }

        [TestMethod()]
        public void ParseAbsoluteUri_ValidInputFullUriStringv1_ValidOutputRequestTarget()
        {
            string uriString;
            RequestTarget expected = BuildFullUriAndRequestTarget("http","user.info","www.test.com",8080,"/","param1=v1", out uriString);

            RequestTargetParser parser = new RequestTargetParser();

            RequestTarget actual = parser.ParseAbsoluteUri(uriString);


            Assert.AreEqual(expected, actual);


        }

        [TestMethod()]
        public void ParseAbsoluteUri_ValidInputFullUriStringv2_ValidOutputRequestTarget()
        {
            string uriString;
            RequestTarget expected = BuildFullUriAndRequestTarget("https", "user2.info345", "www.data.test.com", 443, "/some/path", "p=1&p=2&p=3", out uriString);

            RequestTargetParser parser = new RequestTargetParser();

            RequestTarget actual = parser.ParseAbsoluteUri(uriString);


            Assert.AreEqual(expected, actual);


        }

        [TestMethod()]
        public void ParseAbsoluteUri_ValidInputFullUriStringv3_ValidOutputRequestTarget()
        {
            string uriString;
            RequestTarget expected = BuildFullUriAndRequestTarget("ftp", "ftp.test.client", "ftp.host.test", 223, "//", "p=123", out uriString);

            RequestTargetParser parser = new RequestTargetParser();

            RequestTarget actual = parser.ParseAbsoluteUri(uriString);


            Assert.AreEqual(expected, actual);


        }

        [TestMethod()]
        public void ParseAbsoluteUri_ValidInputFullUriString_AllComponents1CharLength_ValidOutputRequestTarget()
        {
            string uriString;
            RequestTarget expected = BuildFullUriAndRequestTarget("a", "b", "c", 1, "/", "d", out uriString);

            RequestTargetParser parser = new RequestTargetParser();

            RequestTarget actual = parser.ParseAbsoluteUri(uriString);


            Assert.AreEqual(expected, actual);


        }

        [TestMethod()]
        public void ParseAbsoluteUri_ValidInputFullUriString_AllComponents2CharLength_ValidOutputRequestTarget()
        {
            string uriString;
            RequestTarget expected = BuildFullUriAndRequestTarget("aa", "bb", "cc", 11, "//", "dd", out uriString);

            RequestTargetParser parser = new RequestTargetParser();

            RequestTarget actual = parser.ParseAbsoluteUri(uriString);


            Assert.AreEqual(expected, actual);


        }

        [TestMethod()]
        public void ParseAbsoluteUri_ValidInputFullUriString_AllComponents3CharLength_ValidOutputRequestTarget()
        {
            string uriString;
            RequestTarget expected = BuildFullUriAndRequestTarget("aaa", "bbb", "ccc", 111, "/aa", "ddd", out uriString);

            RequestTargetParser parser = new RequestTargetParser();

            RequestTarget actual = parser.ParseAbsoluteUri(uriString);


            Assert.AreEqual(expected, actual);


        }

        [TestMethod()]
        public void ParseAbsoluteUri_ValidFullUriAbsentPort_ValidOutputRequestTarget()
        {
            RequestTarget target = new RequestTarget();
            target.Form = RequestTarget.RequestTargetForm.Absolute;

            target.Host = "w.host.c";
            target.Path = "/";
            target.Port = -1;
            target.Query = "p=1&p=2";
            target.Scheme = "scheme";
            target.UserInfo = "user.info";

            string uriString = "scheme://user.info@w.host.c/?p=1&p=2";
            RequestTargetParser parser = new RequestTargetParser();
            RequestTarget result = parser.ParseAbsoluteUri(uriString);

            Assert.AreEqual(target, result);

        }

        [TestMethod()]
        public void ParseAbsoluteUri_ValidFullUriAbsentPort_AllComponents1CharLength_ValidOutputRequestTarget()
        {
            RequestTarget target = new RequestTarget();
            target.Form = RequestTarget.RequestTargetForm.Absolute;

            target.Host = "w";
            target.Path = "/";
            target.Port = -1;
            target.Query = "p";
            target.Scheme = "s";
            target.UserInfo = "u";

            string uriString = "s://u@w/?p";
            RequestTargetParser parser = new RequestTargetParser();
            RequestTarget result = parser.ParseAbsoluteUri(uriString);

            Assert.AreEqual(target, result);
        }

        [TestMethod()]
        public void ParseAbsoluteUri_ValidFullUriAbsentUserInfo_ValidOutputRequestTarget()
        {
            RequestTarget target = new RequestTarget();
            target.Form = RequestTarget.RequestTargetForm.Absolute;

            target.Host = "host";
            target.Path = "/path";
            target.Port = 801;
            target.Query = "query=value";
            target.Scheme = "https";
            target.UserInfo = string.Empty;

            string uriString = "https://host:801/path?query=value";
            RequestTargetParser parser = new RequestTargetParser();
            RequestTarget result = parser.ParseAbsoluteUri(uriString);

            Assert.AreEqual(target, result);
        }

        [TestMethod()]
        public void ParseAbsoluteUri_ValidFullUriAbsentUserInfoAndPort_ValidOutputRequestTarget()
        {
            RequestTarget target = new RequestTarget();
            target.Form = RequestTarget.RequestTargetForm.Absolute;

            target.Host = "host";
            target.Path = "/path";
            target.Port = -1;
            target.Query = "query=value";
            target.Scheme = "https";
            target.UserInfo = string.Empty;

            string uriString = "https://host/path?query=value";
            RequestTargetParser parser = new RequestTargetParser();
            RequestTarget result = parser.ParseAbsoluteUri(uriString);

            Assert.AreEqual(target, result);
        }

        [TestMethod()]
        public void ParseAbsoluteUri_ValidFullUriAbsentUserInfoAndPort_Components1CharLength_ValidOutputRequestTarget()
        {
            RequestTarget target = new RequestTarget();
            target.Form = RequestTarget.RequestTargetForm.Absolute;

            target.Host = "h";
            target.Path = "/";
            target.Port = -1;
            target.Query = "q";
            target.Scheme = "h";
            target.UserInfo = string.Empty;

            string uriString = "h://h/?q";
            RequestTargetParser parser = new RequestTargetParser();
            RequestTarget result = parser.ParseAbsoluteUri(uriString);

            Assert.AreEqual(target, result);
        }

        public void ParseAbsoluteUri_ValidFullUriAbsentUserInfoAndPortAndQuery_Components1CharLength_ValidOutputRequestTarget()
        {
            RequestTarget target = new RequestTarget();
            target.Form = RequestTarget.RequestTargetForm.Absolute;

            target.Host = "h";
            target.Path = "/";
            target.Port = -1;
            target.Query = string.Empty;
            target.Scheme = "h";
            target.UserInfo = string.Empty;

            string uriString = "h://h/";
            RequestTargetParser parser = new RequestTargetParser();
            RequestTarget result = parser.ParseAbsoluteUri(uriString);

            Assert.AreEqual(target, result);
        }

        //
        //
        // ABSENT AUTHORITY URI TESTS
        // ALL VALID 
        //


        [TestMethod()]
        public void ParseAbsoluteUri_Regularv1UriWithAbsentAuthority_ValidRequestTarget()
        {
            string uriString;
            RequestTarget rt = BuildAbsentAuthorityUriAndRequestTarget("ftp", "/path/to/some/file", "q=1&q=2&q=3", out uriString);
            
            RequestTargetParser parser = new RequestTargetParser();
            RequestTarget result = parser.ParseAbsoluteUri(uriString);

            Assert.AreEqual(rt, result);
        }

        [TestMethod()]
        public void ParseAbsoluteUri_Regularv2UriWithAbsentAuthority_ValidRequestTarget()
        {
            string uriString;
            RequestTarget rt = BuildAbsentAuthorityUriAndRequestTarget("filescheme", "/testpath", "test_query=1&second=2", out uriString);

            RequestTargetParser parser = new RequestTargetParser();
            RequestTarget result = parser.ParseAbsoluteUri(uriString);
            Assert.AreEqual(rt, result);

        }

        [TestMethod()]
        public void ParseAbsoluteUri_Regularv3UriWithAbsentAuthority_ValidRequestTarget()
        {
            string uriString;
            RequestTarget rt = BuildAbsentAuthorityUriAndRequestTarget("https", "/", "q=1&qqq=222", out uriString);

            RequestTargetParser parser = new RequestTargetParser();
            RequestTarget result = parser.ParseAbsoluteUri(uriString);

            Assert.AreEqual(rt, result);
        }

        [TestMethod()]
        public void ParseAbsoluteUri_Regularv4UriWithAbsentAuthority_ValidRequestTarget()
        {
            string uriString;
            RequestTarget rt = BuildAbsentAuthorityUriAndRequestTarget("ldap", "/example/foledr_NAME", "q=123123123123123123", out uriString);

            RequestTargetParser parser = new RequestTargetParser();
            RequestTarget result = parser.ParseAbsoluteUri(uriString);

            Assert.AreEqual(rt, result);
        }

        public void ParseAbsoluteUri_RegularUri1CharLength_ValidOutput()
        {
            string uriString;
            RequestTarget rt = BuildAbsentAuthorityUriAndRequestTarget("a", "/", "q", out uriString);

            RequestTargetParser parser = new RequestTargetParser();
            RequestTarget result = parser.ParseAbsoluteUri(uriString);

            Assert.AreEqual(rt, result);
        }
    }
}
