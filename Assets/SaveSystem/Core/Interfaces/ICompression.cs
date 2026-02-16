namespace SaveSystem.Core
{
    /// <summary>
    /// Strategy interface for compression implementations.
    /// </summary>
    public interface ICompression
    {
        /// <summary>
        /// Compresses data.
        /// </summary>
        /// <param name="data">Data to compress.</param>
        /// <returns>Compressed data.</returns>
        byte[] Compress(byte[] data);
        
        /// <summary>
        /// Decompresses data.
        /// </summary>
        /// <param name="compressedData">Compressed data.</param>
        /// <returns>Decompressed data.</returns>
        byte[] Decompress(byte[] compressedData);
        
        /// <summary>
        /// Gets the compression algorithm name.
        /// </summary>
        string AlgorithmName { get; }
    }
}
