namespace ProtocolEngine.Http.Http1.Protocol
{
    public enum StatusCode
    {
        //100

        Continue = 100,
        SwitchingProtocols = 101,

        //200

        OK = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritativeInformation = 203,
        NoContent = 204,
        ResetContent = 205,

        //300

        MultipleChoices = 300,
        MovedPermanently = 301,
        Found = 302,
        SeeOther = 303,
        UseProxy = 305,
        TemporaryRedirect = 307,

        //400

        BadRequest = 400,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PayloadTooLarge = 413,
        URITooLong = 414,
        UnsupportedMediaType = 415,
        ExpectationFailed = 417,
        UpgradeRequired = 426,


        // 500

        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HTTPVersionNotSupported = 505,
    };

   
}
