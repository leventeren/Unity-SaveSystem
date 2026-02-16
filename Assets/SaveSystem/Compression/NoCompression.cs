using SaveSystem.Core;

namespace SaveSystem.Compression
{
    /// <summary>
    /// Null Object Pattern - No compression implementation.
    /// Pass-through for when compression is not needed.
    /// </summary>
    public class NoCompression : ICompression
    {
        public string AlgorithmName => "None";
        
        public byte[] Compress(byte[] data)
        {
            // Pass-through, no compression
            return data;
        }
        
        public byte[] Decompress(byte[] compressedData)
        {
            // Pass-through, no decompression
            return compressedData;
        }
    }
}
