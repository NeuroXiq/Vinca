using ProtocolEngine.Http.Http1.Protocol;
using System.IO;

namespace ProtocolEngine.Engine.Http1Engine.AbstractLayer
{
    interface IHtdocsSystem
    {
        bool FileExists(string relativePath);
        bool CanRangeStream(string relativePath);
        string GetETag(string fileName, EncodingType encoding);
        long GetLength(string relativePath, EncodingType encoding);
        EncodingType GetEnableEncodings(string relativePath);
        Stream OpenStream(string relativePath, EncodingType encoding);
        void CloseStream(Stream s);
    }
}
