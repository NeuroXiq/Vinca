using ProtocolEngine.Http.Http1.Protocol;
using System;
using System.Collections.Generic;

namespace ProtocolEngine.Http.Http1Map
{
    // TODO StatusCodeMapper is never used. Do somethig with this class.
    // Delete or use in 'ResponseFormatter' to inject string message after status code (?)
    //
    public static class StatusCodeMapper
    {
        static Dictionary<StatusCode, string> statusCodeToInfoMessage;

        static StatusCodeMapper()
        {
            InitializeMapper();
        }

        private static void InitializeMapper()
        {
            statusCodeToInfoMessage = new Dictionary<StatusCode, string>();
            InsertMapStrings();
        }

        private static void InsertMapStrings()
        {
            InsertInformational();
            InsertSuccessful();
            InsertRedirection();
            InsertClientError();
            InsertServerError();
        }

        private static void InsertServerError()
        {
            statusCodeToInfoMessage.Add(StatusCode.NotImplemented, "Method not supported by current version of Vinca");
        }

        private static void InsertClientError()
        {
            
        }

        private static void InsertRedirection()
        {
            
        }

        private static void InsertSuccessful()
        {
            
        }

        private static void InsertInformational()
        {
            
        }

        private static void InsertErrorCodes()
        {
                
        }

        public static string GetMessage(StatusCode statusCode)
        {
            string message = string.Empty;

            if (!statusCodeToInfoMessage.TryGetValue(statusCode, out message))
            {
                //not found, try get default value for this status code

                message = GetDefaultMessage(statusCode);   
            }

            return message;
        }

        private static string GetDefaultMessage(StatusCode statusCode)
        {
            string defaultMessage = Enum.GetName(typeof(StatusCode), statusCode);

            return defaultMessage;
        }
    }
}
