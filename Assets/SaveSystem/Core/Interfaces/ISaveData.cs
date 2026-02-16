namespace SaveSystem.Core
{
    /// <summary>
    /// Marker interface for all saveable data classes.
    /// Provides versioning, validation, and checksum support.
    /// </summary>
    public interface ISaveData
    {
        /// <summary>
        /// Version of the save data format. Used for migration.
        /// </summary>
        int Version { get; }
        
        /// <summary>
        /// Validates the integrity and business rules of the data.
        /// </summary>
        /// <returns>True if data is valid, false otherwise.</returns>
        bool Validate();
        
        /// <summary>
        /// Calculates a checksum for corruption detection.
        /// </summary>
        /// <returns>Checksum string (e.g., CRC32, MD5, SHA256).</returns>
        string GetChecksum();
    }
}
