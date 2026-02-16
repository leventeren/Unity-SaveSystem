using SaveSystem.Core;
using SaveSystem.Serialization;
using SaveSystem.Storage;
using SaveSystem.Features.Backup;
using SaveSystem.Features.AutoSave;
using SaveSystem.Utilities;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// Fluent builder for configuring SaveManager.
    /// Implements Builder pattern for easy and readable configuration.
    /// </summary>
    public class SaveManagerBuilder
    {
        private ISerializer _serializer;
        private ISaveStrategy _saveStrategy;
        private BackupManager _backupManager;
        private AutoSaveManager _autoSaveManager;
        private int _maxBackupCount = 3;
        private float _autoSaveInterval = 300f;  // 5 minutes
        
        public SaveManagerBuilder()
        {
            // Defaults
            _serializer = new JsonSerializer();
            _saveStrategy = new LocalFileSaveStrategy();
        }
        
        /// <summary>
        /// Use JSON serialization (Unity JsonUtility).
        /// </summary>
        public SaveManagerBuilder UseJsonSerializer()
        {
            _serializer = new JsonSerializer();
            return this;
        }
        
        /// <summary>
        /// Use Binary serialization.
        /// </summary>
        public SaveManagerBuilder UseBinarySerializer()
        {
            _serializer = new BinarySerializer();
            return this;
        }
        
        /// <summary>
        /// Use custom serializer.
        /// </summary>
        public SaveManagerBuilder UseSerializer(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }
        
        /// <summary>
        /// Builds the SaveManager with configured settings.
        /// </summary>
        public SaveManager Build()
        {
            if (_serializer == null)
                throw new System.InvalidOperationException("Serializer must be configured");
            
            if (_saveStrategy == null)
                throw new System.InvalidOperationException("Save strategy must be configured");
            
            // Build SaveManager first
            var saveManager = new SaveManager(_serializer, _saveStrategy);
            
            // Configure optional features
            BackupManager backupManager = null;
            AutoSaveManager autoSaveManager = null;
            
            if (_maxBackupCount > 0)
            {
                backupManager = new BackupManager(saveManager, _maxBackupCount);
            }
            
            if (_autoSaveInterval > 0)
            {
                autoSaveManager = new AutoSaveManager(saveManager, _autoSaveInterval);
            }
            
            // Rebuild SaveManager with features
            var finalSaveManager = new SaveManager(_serializer, _saveStrategy, backupManager, autoSaveManager);
            
            // Auto-register for updates if AutoSave is enabled
            if (autoSaveManager != null)
            {
                var updateProxy = SaveManagerUpdateProxy.GetOrCreate();
                updateProxy.RegisterSaveManager(finalSaveManager);
            }
            
            return finalSaveManager;
        }
        
        /// <summary>
        /// Use local file storage.
        /// </summary>
        public SaveManagerBuilder UseLocalFileStorage(string basePath = null)
        {
            string extension = _serializer?.FileExtension ?? ".save";
            _saveStrategy = new LocalFileSaveStrategy(extension, basePath);
            return this;
        }
        
        /// <summary>
        /// Use PlayerPrefs storage.
        /// </summary>
        public SaveManagerBuilder UsePlayerPrefsStorage()
        {
            _saveStrategy = new PlayerPrefsSaveStrategy();
            return this;
        }
        
        /// <summary>
        /// Enable automatic backup creation after each save.
        /// </summary>
        public SaveManagerBuilder WithBackup(int maxBackupCount = 3)
        {
            _maxBackupCount = maxBackupCount;
            return this;
        }
        
        /// <summary>
        /// Enable automatic saving at specified interval.
        /// </summary>
        public SaveManagerBuilder WithAutoSave(float intervalSeconds = 300f)
        {
            _autoSaveInterval = intervalSeconds;
            return this;
        }
        
        /// <summary>
        /// Use custom storage strategy.
        /// </summary>
        public SaveManagerBuilder UseStorage(ISaveStrategy saveStrategy)
        {
            _saveStrategy = saveStrategy;
            return this;
        }
        
        /// <summary>
        /// Auto-detect platform and use optimized settings.
        /// Editor: LocalFile (for debugging)
        /// Mobile Device: PlayerPrefs
        /// PC Build: LocalFile
        /// </summary>
        public SaveManagerBuilder UseAutoDetectPlatform()
        {
#if UNITY_EDITOR
            // Always use LocalFile in editor for easy debugging
            return UsePCPreset();
#elif UNITY_ANDROID || UNITY_IOS
            // Use PlayerPrefs on actual mobile devices
            return UseMobilePreset();
#else
            // PC builds
            return UsePCPreset();
#endif
        }
        
        /// <summary>
        /// Use mobile-optimized preset configuration.
        /// PlayerPrefs storage + JSON serialization (lightweight).
        /// </summary>
        public SaveManagerBuilder UseMobilePreset()
        {
            SaveSystemLogger.Log("Using Mobile Preset: PlayerPrefs + JSON");
            
            _serializer = new JsonSerializer();
            _saveStrategy = new PlayerPrefsSaveStrategy();
            
            return this;
        }
        
        /// <summary>
        /// Use PC-optimized preset configuration.
        /// LocalFile storage + JSON serialization.
        /// </summary>
        public SaveManagerBuilder UsePCPreset()
        {
            SaveSystemLogger.Log("Using PC Preset: LocalFile + JSON");
            
            _serializer = new JsonSerializer();
            string extension = _serializer.FileExtension;
            _saveStrategy = new LocalFileSaveStrategy(extension);
            
            return this;
        }
        
        /// <summary>
        /// Use settings from ScriptableObject.
        /// This is the recommended way to configure SaveSystem.
        /// </summary>
        public SaveManagerBuilder UseSettings(SaveSystemSettings settings)
        {
            if (settings == null)
            {
                SaveSystemLogger.LogError("Settings is null! Using default configuration.");
                return UseAutoDetectPlatform();
            }
            
            settings.ValidateSettings();
            
            if (settings.verboseLogging)
            {
                SaveSystemLogger.Log($"Applying settings: {settings.name}");
            }
            
            // Apply serializer
            switch (settings.serializerType)
            {
                case SerializerType.JSON:
                    UseJsonSerializer();
                    break;
                case SerializerType.Binary:
                    UseBinarySerializer();
                    break;
            }
            
            // Apply storage
            switch (settings.storageType)
            {
                case StorageType.AutoDetect:
                    UseAutoDetectPlatform();
                    break;
                case StorageType.LocalFile:
                    UseLocalFileStorage();
                    break;
                case StorageType.PlayerPrefs:
                    UsePlayerPrefsStorage();
                    break;
            }
            
            // Apply compression
            // Note: Compression is applied at the serialization level in the future
            
            // Apply backup settings
            if (settings.enableBackup)
            {
                WithBackup(settings.maxBackupCount);
            }
            
            // Apply autosave settings
            if (settings.enableAutoSave)
            {
                WithAutoSave(settings.autoSaveInterval);
            }
            
            if (settings.verboseLogging)
            {
                SaveSystemLogger.LogSuccess($"Configuration complete - Serializer: {settings.serializerType}, Storage: {settings.storageType}");
            }
            
            return this;
        }
    }
}
