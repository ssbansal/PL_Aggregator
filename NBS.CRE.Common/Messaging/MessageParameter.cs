using System;

namespace NBS.CRE.Common.Messaging
{
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

        public string Name { get; private set; }
        public object Value { get; private set; }
    }
}
