using System;

namespace NBS.CRE.Common.Messaging
{
    /// <summary>
    /// Message Parameter
    /// </summary>
    [Serializable]
    public class MessageParameter
    {
        public MessageParameter(string name, object value)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Value = value;
        }

        /// <value>Parameter name.</value>
        public string Name { get; private set; }
        
        /// <value>Parameter value.</value>
        public object Value { get; private set; }
    }
}
