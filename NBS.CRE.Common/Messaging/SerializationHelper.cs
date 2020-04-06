using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NBS.CRE.Common.Messaging
{

    /// <summary>
    /// Binary serialization helper.
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// Serialize object to bytes array.
        /// </summary>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>Object serialized to <c>byte[]</c>.</returns>
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

        /// <summary>
        /// Deserialize object for bytes array.
        /// </summary>
        /// <typeparam name="T">Target Object</typeparam>
        /// <param name="bytes">Serialized bytes array.</param>
        /// <returns>Deserialized object instance.</returns>
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
