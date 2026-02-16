using SaveSystem.Core;
using System;

namespace SaveSystem.Serialization
{
    /// <summary>
    /// Abstract base class for serializers.
    /// Implements Template Method pattern for common serialization behavior.
    /// </summary>
    public abstract class BaseSerializer : ISerializer
    {
        public abstract string FileExtension { get; }
        
        public string Serialize<T>(T data) where T : ISaveData
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            
            // Validate before serialization
            if (!data.Validate())
            {
                throw new InvalidOperationException("Data validation failed before serialization.");
            }
            
            try
            {
                return SerializeInternal(data);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Serialization failed: {ex.Message}", ex);
            }
        }
        
        public T Deserialize<T>(string serializedData) where T : ISaveData
        {
            if (string.IsNullOrEmpty(serializedData))
            {
                throw new ArgumentNullException(nameof(serializedData));
            }
            
            try
            {
                T result = DeserializeInternal<T>(serializedData);
                
                // Validate after deserialization
                if (result == null || !result.Validate())
                {
                    throw new InvalidOperationException("Data validation failed after deserialization.");
                }
                
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Deserialization failed: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Implement actual serialization logic in derived classes.
        /// </summary>
        protected abstract string SerializeInternal<T>(T data) where T : ISaveData;
        
        /// <summary>
        /// Implement actual deserialization logic in derived classes.
        /// </summary>
        protected abstract T DeserializeInternal<T>(string serializedData) where T : ISaveData;
    }
}
