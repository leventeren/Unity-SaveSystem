using System.Threading.Tasks;

namespace SaveSystem.Core
{
    /// <summary>
    /// Strategy interface for serialization implementations.
    /// Supports different formats (JSON, Binary, XML, etc.).
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes data to string or byte array.
        /// </summary>
        /// <typeparam name="T">Type of data to serialize.</typeparam>
        /// <param name="data">Data to serialize.</param>
        /// <returns>Serialized string representation.</returns>
        string Serialize<T>(T data) where T : ISaveData;
        
        /// <summary>
        /// Deserializes data from string or byte array.
        /// </summary>
        /// <typeparam name="T">Type of data to deserialize.</typeparam>
        /// <param name="serializedData">Serialized data string.</param>
        /// <returns>Deserialized data object.</returns>
        T Deserialize<T>(string serializedData) where T : ISaveData;
        
        /// <summary>
        /// Gets the file extension for this serialization format.
        /// </summary>
        string FileExtension { get; }
    }
}
