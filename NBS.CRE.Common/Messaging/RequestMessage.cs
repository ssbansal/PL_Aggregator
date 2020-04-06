using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Linq;

namespace NBS.CRE.Common.Messaging
{
    /// <summary>
    /// Request Message
    /// </summary>
    [Serializable]
    public class RequestMessage : ISerializable
    {
        public RequestMessage(string actionCode, XElement data, IEnumerable<MessageParameter> parameters = null)
        {
            if (String.IsNullOrEmpty(actionCode))
            {
                throw new ArgumentNullException(nameof(actionCode));
            }

            ActionCode = actionCode;
            Data = data;
            Parameters = new MessageParameter[0];

            if (parameters != null)
            {
                var paramsList = new List<MessageParameter>();
                paramsList.AddRange(parameters);
                Parameters = paramsList.ToArray();
            }
        }

        protected RequestMessage(SerializationInfo info, StreamingContext context)
        {
            ActionCode = info.GetString("ActionCode");
            Parameters = (MessageParameter[])info.GetValue("Parameters", typeof(MessageParameter[]));

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

        /// <value>Action code used for routing by <see cref="RPC.RpcClient"/>.</value>
        public string ActionCode { get; private set; }

        /// <value>Request parameters array.</value>
        public MessageParameter[] Parameters { get; private set; }

        /// <value>Request XML data.</value>
        public XElement Data { get; private set; }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ActionCode", ActionCode);
            info.AddValue("Parameters", Parameters);

            string xmlData = String.Empty;
            if (Data != null)
            {
                xmlData = Data.ToString();
            }
            info.AddValue("Data", xmlData);
        }
    }
}
