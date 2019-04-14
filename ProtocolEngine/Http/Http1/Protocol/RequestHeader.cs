using ProtocolEngine.Http.Http1.Protocol;
using System;
using System.Collections.Generic;

namespace Vinca.ProtocolEngine.Http1.Protocol
{
    public class RequestHeader
    {
        //HeaderFieldsContainer headerFieldsContainer;
        Dictionary<HFType, IHeaderField> onlySingleHeaderFields;

        private List<UndefinedHf> undefinedHfList;
        private List<IHeaderField> cookieHfList;

        public HttpMethod Method { get; private set; }
        public RequestTarget Target { get; private set; }
        public HttpVersion Version { get; private set; }

        public RequestHeader(HttpMethod method, RequestTarget target, HttpVersion version, IHeaderField[] headerFields)
        {
            Method = method;
            Target = target;
            Version = version;

            undefinedHfList = new List<UndefinedHf>();
            cookieHfList = new List<IHeaderField>();
            onlySingleHeaderFields = new Dictionary<HFType, IHeaderField>();

            AddFields(headerFields);
        }

        public void AddFields(params IHeaderField[] headerFields)
        {
            //headerFieldsContainer.AddRange(headerFields);
            foreach (IHeaderField hf in headerFields)
            {
                //
                // Two special cases of header fields:
                // 'Set-Cookie:' field can appear several times (http/1.1 specification)
                // and this is expected and 'normal'.
                //
                // 'UndefinedHf' struct holds key-value from header field which parsing method is undefined now.
                // this speciaf header field struct contains two strings 'Key' & 'Value' parsed exactly to string.

                switch (hf.Type)
                {
                    case HFType.Undefined:
                        SpecialAddToUndefined((UndefinedHf)hf);
                        break;
                    case HFType.Cookie:
                        SpecialAddToCookie(hf);
                        break;
                    default:
                        AddOnlySingleHeaderField(hf);
                        break;
                }
            }
        }

        ///<summary>Indicates if at least one field of '<param name="type"/>' type is present in the header</summary>
        ///<param name="type">Type of header field</param>
        public bool Contains(HFType type)
        {
            if (HFType.Undefined == type)
                return undefinedHfList.Count > 0;
            else if (HFType.Cookie == type)
                return cookieHfList.Count > 0;
            else return onlySingleHeaderFields.ContainsKey(type);
        }

        public T GetSingleField<T>(HFType type) where T : IHeaderField
        {
            if (type == HFType.Undefined || type == HFType.Cookie)
            {
                string msg = string.Format("Cannot get no-single field as single one."+
                                           " Use 'GetUndefinedField()' or 'GetCookieFields()' as needed");
                throw new InvalidOperationException(msg);
            }

            return (T)onlySingleHeaderFields[type];
        }

        public UndefinedHf[] GetUndefinedFields()
        {
            return undefinedHfList.ToArray();
        }

        public IHeaderField[] GetCookieFields()
        {
            return cookieHfList.ToArray();
        }

        private void AddOnlySingleHeaderField(IHeaderField hf)
        {
            //error is unexpected and message format is invalid
            onlySingleHeaderFields.Add(hf.Type, hf);
        }

        private void SpecialAddToCookie(IHeaderField hf)
        {
            cookieHfList.Add(hf);
        }

        private void SpecialAddToUndefined(UndefinedHf hf)
        {
            undefinedHfList.Add(hf);
        }

        //public bool TryGet<T>(HFType type, out T field) where T : IHeaderField
        //{
        //    IHeaderField headerField;
        //    bool found = false;
        //
        //    found = false; headerFieldsContainer.TryGetFirst(type, out headerField);
        //    if (found)
        //        field = (T)headerField;
        //    else field = default(T);
        //
        //
        //    return found;
        //}
    }
}
