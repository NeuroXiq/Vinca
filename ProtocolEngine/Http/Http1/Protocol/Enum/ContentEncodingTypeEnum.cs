using System;

namespace ProtocolEngine.Http.Http1.Protocol
{
    // Copy paste from: 
    //https://en.wikipedia.org/wiki/HTTP_compression
    //
    // All possible encoding should be present in this enum.
    // Parsing require all possible names of 'content-encoding' request header field
    // 

    [Flags]
    public enum EncodingType : int
    {
        ///<summary>Special value indicates empty encoding.</summary>
        NULL         = (0 << 0),
        ///<summary>No encoding</summary>
        Identity     = (1 << 0),
        ///<summary> '*' Value indicates to use any encoding type </summary>
        Any = (1 << 1),
        Compress     = (1 << 2),
        Deflate      = (1 << 3),
        Exi          = (1 << 4),
        Gzip         = (1 << 5),
        Pack200_gzip = (1 << 6),
        Zstd         = (1 << 7),
        Bzip2        = (1 << 8),
        Izma         = (1 << 9),
        Peerdist     = (1 << 10),
        Sdch         = (1 << 11),
        Xpress       = (1 << 12),
        Xz           = (1 << 13),
        Br           = (1 << 14)
    }
}
