using System;
using System.IO;
using ProtocolEngine.Engine.Http1Engine.AbstractLayer;
using ProtocolEngine.Http.Http1.Protocol;
using System.Security.Cryptography;
using System.Text;

namespace ProtocolEngine.HtdocsResourcesManagement.HtdocsSystem
{
    ///<summary>Default htdocs system. Operates only on source htdocs files, no compression, no cache</summary>
    class HtdocsDefaultSystem : IHtdocsSystem
    {
        PathResolver pathResolver;

        public HtdocsDefaultSystem(PathResolver pathResolver)
        {
            this.pathResolver = pathResolver;
        }

        public bool CanRangeStream(string relativePath)
        {
            return true;
        }

        public void CloseStream(Stream s)
        {
            s.Close();
        }

        public bool FileExists(string relativePath)
        {
            string fileName = pathResolver.ToAbsolute(relativePath);
            return File.Exists(fileName);
        }

        public EncodingType GetEnableEncodings(string relativePath)
        {
            return EncodingType.Identity;
        }

        public string GetETag(string relativePath, EncodingType encoding)
        {
            ThrowIfNotIdentity(encoding);

            string fileName = pathResolver.ToAbsolute(relativePath);
            MD5 md5 = MD5.Create();
            FileStream fStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] hashBytes = md5.ComputeHash(fStream);
            fStream.Close();

            return Encoding.ASCII.GetString(hashBytes);
        }

        public long GetLength(string relativePath, EncodingType encoding)
        {
            ThrowIfNotIdentity(encoding);

            string fileName = pathResolver.ToAbsolute(relativePath);
            return new FileInfo(fileName).Length;
        }

        public Stream OpenStream(string relativePath, EncodingType encoding)
        {
            ThrowIfNotIdentity(encoding);
            string fileName = pathResolver.ToAbsolute(relativePath);
            return new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private void ThrowIfNotIdentity(EncodingType encoding)
        {
            if (encoding != EncodingType.Identity) throw new NotSupportedException("Encoding not supported. Only 'Identity' allowed");
        }
    }
}
