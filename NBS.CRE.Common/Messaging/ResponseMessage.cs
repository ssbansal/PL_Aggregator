using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Linq;

namespace NBS.CRE.Common.Messaging
{
    [Serializable]
    public class ResponseMessage : ISerializable
    {
        private ResponseMessage(XElement data)
        {
            IsError = false;
            Data = data;
        }

        private ResponseMessage(Exception error)
        {
            IsError = true;
            Error = error;
        }

        public static ResponseMessage CreateResponseOK(XElement data)
        {
            return new ResponseMessage(data);
        }

        public static ResponseMessage CreateResponseERROR(Exception ex)
        {
            return new ResponseMessage(ex);
        }
        protected ResponseMessage(SerializationInfo info, StreamingContext context)
        {
            IsError = info.GetBoolean("IsError");
            Error = info.GetValue("Error", typeof(Exception)) as Exception;

            string xmlData = info.GetString("Data");
            if (!String.IsNullOrEmpty(xmlData))
            {
                Data = XElement.Parse(xmlData);
            }
            else
            {
                Data = null;
            }

        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsError", IsError);
            info.AddValue("Error", Error);

            string xmlData = String.Empty;
            if (Data != null)
            {
                xmlData = Data.ToString();
            }
            info.AddValue("Data", xmlData);
        }

        public bool IsError { get; private set; }
        public Exception Error { get; private set; }
        public XElement Data { get; private set; }
    }
}
