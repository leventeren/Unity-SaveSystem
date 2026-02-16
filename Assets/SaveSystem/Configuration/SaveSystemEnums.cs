namespace SaveSystem
{
    /// <summary>
    /// Serializer type options.
    /// </summary>
    public enum SerializerType
    {
        JSON,
        Binary
    }
    
    /// <summary>
    /// Storage type options.
    /// </summary>
    public enum StorageType
    {
        AutoDetect,
        LocalFile,
        PlayerPrefs
    }
    
    /// <summary>
    /// Checksum algorithm options.
    /// </summary>
    public enum ChecksumAlgorithm
    {
        CRC32,
        MD5,
        SHA256
    }
}
