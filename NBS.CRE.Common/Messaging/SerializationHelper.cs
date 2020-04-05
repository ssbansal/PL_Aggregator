using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NBS.CRE.Common.Messaging
{
    public static class SerializationHelper
    {
        public static byte[] ToBytes(object obj)
        {
            byte[] result = default(byte[]);
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                result = stream.GetBuffer();
            }

            return result;
        }

        public static T FromBytes<T>(byte[] bytes) where T : class
        {
            T result = default(T);
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                result = formatter.Deserialize(stream) as T;
            }

            return result;
        }
    }
}
