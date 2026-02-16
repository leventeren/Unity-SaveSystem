using SaveSystem.Core;
using UnityEngine;

namespace SaveSystem.Serialization
{
    /// <summary>
    /// JSON serializer using Unity's JsonUtility.
    /// Simple and lightweight, but has limitations with Dictionary and some complex types.
    /// </summary>
    public class JsonSerializer : BaseSerializer
    {
        public override string FileExtension => ".json";
        
        protected override string SerializeInternal<T>(T data)
        {
            // Unity's JsonUtility.ToJson with pretty print for debugging
            return JsonUtility.ToJson(data, prettyPrint: true);
        }
        
        protected override T DeserializeInternal<T>(string serializedData)
        {
            return JsonUtility.FromJson<T>(serializedData);
        }
    }
}
