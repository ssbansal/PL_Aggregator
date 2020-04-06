using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Linq;

namespace NBS.CRE.Common.Messaging
{
    /// <summary>
    /// Response Message
    /// </summary>
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

        /// <summary>
        /// Helper method to create a success response.
        /// </summary>
        /// <param name="data">Response data.</param>
        /// <returns><see cref="ResponseMessage"/> object.</returns>
        public static ResponseMessage CreateResponseOK(XElement data)
        {
            return new ResponseMessage(data);
        }

        /// <summary>
        /// Helper method to generate a failure response.
        /// </summary>
        /// <param name="ex">Exception object.</param>
        /// <returns><see cref="ResponseMessage"/> object.</returns>
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

        /// <value>Return <c>true</c> when response message is in error state.</value>
        public bool IsError { get; private set; }

        /// <value>Exception object.</value>
        public Exception Error { get; private set; }

        /// <value>XML data object.</value>
        public XElement Data { get; private set; }
    }
}
