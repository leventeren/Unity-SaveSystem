using System;

namespace SaveSystem.Models
{
    /// <summary>
    /// Metadata for save files.
    /// Tracks version, timestamp, platform, and integrity information.
    /// </summary>
    [Serializable]
    public class SaveMetadata
    {
        /// <summary>
        /// Save data format version.
        /// </summary>
        public int Version { get; set; }
        
        /// <summary>
        /// When the save was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// When the save was last modified (UTC).
        /// </summary>
        public DateTime ModifiedAt { get; set; }
        
        /// <summary>
        /// Platform the save was created on.
        /// </summary>
        public string Platform { get; set; }
        
        /// <summary>
        /// Checksum for corruption detection.
        /// </summary>
        public string Checksum { get; set; }
        
        /// <summary>
        /// Total play time in seconds.
        /// </summary>
        public float TotalPlayTime { get; set; }
        
        /// <summary>
        /// Custom metadata key-value pairs.
        /// </summary>
        public SerializableDictionary<string, string> CustomData { get; set; }
        
        public SaveMetadata()
        {
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
            Platform = UnityEngine.Application.platform.ToString();
            CustomData = new SerializableDictionary<string, string>();
        }
    }
}
