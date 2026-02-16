using SaveSystem.Core;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SaveSystem.Examples
{
    /// <summary>
    /// Example player data with simple primitive types.
    /// Demonstrates ISaveData implementation with validation and checksum.
    /// </summary>
    [Serializable]
    public class PlayerData : ISaveData
    {
        public int Version => 1;
        
        public string Name;
        public int Level;
        public int Gold;
        public float Experience;
        public int Health;
        public int MaxHealth;
        
        public PlayerData()
        {
            Name = "Player";
            Level = 1;
            Gold = 0;
            Experience = 0f;
            Health = 100;
            MaxHealth = 100;
        }
        
        public bool Validate()
        {
            // Business rule validation
            if (string.IsNullOrEmpty(Name))
                return false;
            
            if (Level < 1 || Level > 100)
                return false;
            
            if (Gold < 0)
                return false;
            
            if (Health < 0 || Health > MaxHealth)
                return false;
            
            return true;
        }
        
        public string GetChecksum()
        {
            // Simple checksum using MD5
            string data = $"{Name}{Level}{Gold}{Experience}{Health}{MaxHealth}";
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
