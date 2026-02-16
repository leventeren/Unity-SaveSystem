using SaveSystem.Core;
using System;
using System.IO;
using System.IO.Compression;

namespace SaveSystem.Compression
{
    /// <summary>
    /// GZip compression implementation.
    /// Provides 60-80% file size reduction with moderate CPU overhead.
    /// </summary>
    public class GZipCompression : ICompression
    {
        public string AlgorithmName => "GZip";
        
        public byte[] Compress(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Data cannot be null or empty.", nameof(data));
            }
            
            using (var outputStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    gzipStream.Write(data, 0, data.Length);
                }
                
                return outputStream.ToArray();
            }
        }
        
        public byte[] Decompress(byte[] compressedData)
        {
            if (compressedData == null || compressedData.Length == 0)
            {
                throw new ArgumentException("Compressed data cannot be null or empty.", nameof(compressedData));
            }
            
            using (var inputStream = new MemoryStream(compressedData))
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var outputStream = new MemoryStream())
            {
                gzipStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }
    }
}
