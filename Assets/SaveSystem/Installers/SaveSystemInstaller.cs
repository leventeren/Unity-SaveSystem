using SaveSystem;
using SaveSystem.Core;
using SaveSystem.Serialization;
using SaveSystem.Storage;
using SaveSystem.Compression;
using SaveSystem.Features.AutoSave;
using SaveSystem.Features.Backup;
using SaveSystem.Features.Validation;
using SaveSystem.Features.Versioning;
using UnityEngine;
using Zenject;

namespace SaveSystem.Installers
{
    /// <summary>
    /// Zenject installer for SaveSystem.
    /// Binds all SaveSystem components to the DI container.
    /// </summary>
    public class SaveSystemInstaller : MonoInstaller
    {
        [Header("Configuration")]
        [SerializeField] private bool useJsonSerializer = true;
        [SerializeField] private bool useBinarySerializer = false;
        [SerializeField] private bool useCompression = false;
        [SerializeField] private bool enableAutoSave = true;
        [SerializeField] private float autoSaveInterval = 300f;
        [SerializeField] private int maxBackupCount = 3;
        
        public override void InstallBindings()
        {
            BindSerializer();
            BindSaveStrategy();
            BindCompression();
            BindSaveManager();
            BindAdvancedFeatures();
        }
        
        private void BindSerializer()
        {
            // Ensure at least one serializer is selected
            if (!useJsonSerializer && !useBinarySerializer)
            {
                UnityEngine.Debug.LogWarning("No serializer selected! Defaulting to JSON serializer.");
                useJsonSerializer = true;
            }
            
            // Prefer binary if both are selected
            if (useBinarySerializer)
            {
                Container.Bind<ISerializer>()
                    .To<BinarySerializer>()
                    .AsSingle();
            }
            else if (useJsonSerializer)
            {
                Container.Bind<ISerializer>()
                    .To<JsonSerializer>()
                    .AsSingle();
            }
        }
        
        private void BindSaveStrategy()
        {
            // You can bind different strategies based on platform or config
            Container.Bind<ISaveStrategy>()
                .To<LocalFileSaveStrategy>()
                .AsSingle()
                .WithArguments(".json", (string)null); // fileExtension, basePath
        }
        
        private void BindCompression()
        {
            if (useCompression)
            {
                Container.Bind<ICompression>()
                    .To<GZipCompression>()
                    .AsSingle();
            }
            else
            {
                Container.Bind<ICompression>()
                    .To<NoCompression>()
                    .AsSingle();
            }
        }
        
        private void BindSaveManager()
        {
            // SaveManager as singleton
            Container.Bind<SaveManager>()
                .AsSingle();
        }
        
        private void BindAdvancedFeatures()
        {
            // AutoSaveManager
            if (enableAutoSave)
            {
                Container.Bind<AutoSaveManager>()
                    .AsSingle()
                    .WithArguments(autoSaveInterval);
            }
            
            // BackupManager
            Container.Bind<BackupManager>()
                .AsSingle()
                .WithArguments(maxBackupCount, (string)null); // maxBackupCount, backupBasePath
            
            // CorruptionDetector
            Container.Bind<CorruptionDetector>()
                .AsSingle()
                .WithArguments(CorruptionDetector.ChecksumAlgorithm.MD5);
            
            // VersionMigrationManager
            Container.Bind<VersionMigrationManager>()
                .AsSingle();
        }
    }
}
