using UnityEngine;
using SaveSystem.Features.Validation;
using SaveSystem.Utilities;

namespace SaveSystem
{
    /// <summary>
    /// ScriptableObject-based configuration for SaveSystem.
    /// Allows easy project-wide settings management through Inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "SaveSystemSettings", menuName = "SaveSystem/Settings", order = 1)]
    public class SaveSystemSettings : ScriptableObject
    {
        [Header("Profile Settings")]
        [Tooltip("Default save profile name")]
        public string defaultProfileName = "Player";
        
        [Header("Serialization")]
        [Tooltip("Serializer to use for save data")]
        public SerializerType serializerType = SerializerType.JSON;
        
        [Header("Storage")]
        [Tooltip("Storage strategy (AutoDetect recommended)")]
        public StorageType storageType = StorageType.AutoDetect;
        
        [Header("Auto-Save")]
        [Tooltip("Enable automatic saving at intervals")]
        public bool enableAutoSave = true;
        
        [Tooltip("Auto-save interval in seconds")]
        [Range(30f, 600f)]
        public float autoSaveInterval = 300f; // 5 minutes
        
        [Header("Backup")]
        [Tooltip("Enable automatic backup creation")]
        public bool enableBackup = true;
        
        [Tooltip("Maximum number of backups to keep")]
        [Range(1, 10)]
        public int maxBackupCount = 3;
        
        [Header("Compression")]
        [Tooltip("Enable GZip compression (60-80% size reduction)")]
        public bool useCompression = false;
        
        [Header("Corruption Detection")]
        [Tooltip("Checksum algorithm for data integrity")]
        public CorruptionDetector.ChecksumAlgorithm checksumAlgorithm = CorruptionDetector.ChecksumAlgorithm.MD5;
        
        [Header("Debug")]
        [Tooltip("Enable verbose logging")]
        public bool verboseLogging = false;
        
        /// <summary>
        /// Validates settings and logs warnings for invalid configurations.
        /// </summary>
        public void ValidateSettings()
        {
            // Apply verbose logging setting
            SaveSystemLogger.VerboseLogging = verboseLogging;
            
            if (enableAutoSave && autoSaveInterval < 30f)
            {
                SaveSystemLogger.LogWarning("Auto-save interval is very short. Consider increasing to at least 60 seconds.");
            }
            
            if (enableBackup && maxBackupCount < 1)
            {
                SaveSystemLogger.LogWarning("Max backup count must be at least 1. Setting to 1.");
                maxBackupCount = 1;
            }
            
            if (string.IsNullOrEmpty(defaultProfileName))
            {
                SaveSystemLogger.LogWarning("Default profile name is empty. Using 'Player'.");
                defaultProfileName = "Player";
            }
        }
    }
}
