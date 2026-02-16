using SaveSystem.Core;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SaveSystem.Serialization
{
    /// <summary>
    /// Binary serializer for compact storage.
    /// Uses BinaryFormatter (or can be replaced with modern alternatives like MessagePack).
    /// </summary>
    public class BinarySerializer : BaseSerializer
    {
        public override string FileExtension => ".sav";
        
        protected override string SerializeInternal<T>(T data)
        {
            #pragma warning disable SYSLIB0011 // BinaryFormatter is obsolete
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, data);
                byte[] bytes = memoryStream.ToArray();
                // Convert to base64 string for consistency with string-based API
                return Convert.ToBase64String(bytes);
            }
            #pragma warning restore SYSLIB0011
        }
        
        protected override T DeserializeInternal<T>(string serializedData)
        {
            #pragma warning disable SYSLIB0011 // BinaryFormatter is obsolete
            byte[] bytes = Convert.FromBase64String(serializedData);
            using (var memoryStream = new MemoryStream(bytes))
            {
                var formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(memoryStream);
            }
            #pragma warning restore SYSLIB0011
        }
    }
}
