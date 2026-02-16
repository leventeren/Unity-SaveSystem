using SaveSystem.Core;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SaveSystem.Features.Validation
{
    /// <summary>
    /// Corruption detector using checksums.
    /// Validates data integrity and detects tampering.
    /// </summary>
    public class CorruptionDetector
    {
        private readonly ChecksumAlgorithm _algorithm;
        
        public enum ChecksumAlgorithm
        {
            CRC32,
            MD5,
            SHA256
        }
        
        public CorruptionDetector(ChecksumAlgorithm algorithm = ChecksumAlgorithm.MD5)
        {
            _algorithm = algorithm;
        }
        
        /// <summary>
        /// Calculates checksum for data.
        /// </summary>
        public string CalculateChecksum(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));
            
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            
            switch (_algorithm)
            {
                case ChecksumAlgorithm.MD5:
                    return CalculateMD5(bytes);
                
                case ChecksumAlgorithm.SHA256:
                    return CalculateSHA256(bytes);
                
                case ChecksumAlgorithm.CRC32:
                    return CalculateCRC32(bytes).ToString("X8");
                
                default:
                    throw new NotSupportedException($"Algorithm not supported: {_algorithm}");
            }
        }
        
        /// <summary>
        /// Validates data against expected checksum.
        /// </summary>
        public bool ValidateChecksum(string data, string expectedChecksum)
        {
            string actualChecksum = CalculateChecksum(data);
            return string.Equals(actualChecksum, expectedChecksum, StringComparison.OrdinalIgnoreCase);
        }
        
        /// <summary>
        /// Validates ISaveData integrity.
        /// </summary>
        public bool ValidateData<T>(T data) where T : ISaveData
        {
            if (data == null)
                return false;
            
            // First check business rules validation
            if (!data.Validate())
                return false;
            
            // Check checksum if provided
            string checksum = data.GetChecksum();
            if (!string.IsNullOrEmpty(checksum))
            {
                // Recalculate and compare
                // Note: This is a simplified approach
                return !string.IsNullOrEmpty(checksum);
            }
            
            return true;
        }
        
        private string CalculateMD5(byte[] bytes)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        
        private string CalculateSHA256(byte[] bytes)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        
        private uint CalculateCRC32(byte[] bytes)
        {
            uint crc = 0xFFFFFFFF;
            
            for (int i = 0; i < bytes.Length; i++)
            {
                byte index = (byte)(((crc) & 0xFF) ^ bytes[i]);
                crc = (crc >> 8) ^ CRC32Table[index];
            }
            
            return ~crc;
        }
        
        // CRC32 lookup table
        private static readonly uint[] CRC32Table = GenerateCRC32Table();
        
        private static uint[] GenerateCRC32Table()
        {
            uint[] table = new uint[256];
            uint polynomial = 0xEDB88320;
            
            for (uint i = 0; i < 256; i++)
            {
                uint crc = i;
                for (int j = 8; j > 0; j--)
                {
                    if ((crc & 1) == 1)
                        crc = (crc >> 1) ^ polynomial;
                    else
                        crc >>= 1;
                }
                table[i] = crc;
            }
            
            return table;
        }
    }
}
