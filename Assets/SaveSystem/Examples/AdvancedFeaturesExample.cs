using SaveSystem.Features.AutoSave;
using SaveSystem.Features.Backup;
using SaveSystem.Models;
using System.Collections.Generic;
using UnityEngine;
using SaveSystem.Utilities;

namespace SaveSystem.Examples
{
    /// <summary>
    /// Example demonstrating auto-save and backup features.
    /// </summary>
    public class AdvancedFeaturesExample : MonoBehaviour
    {
        private SaveManager _saveManager;
        private AutoSaveManager _autoSaveManager;
        private BackupManager _backupManager;
        
        private PlayerData _playerData;
        
        private void Start()
        {
            // Setup SaveManager
            _saveManager = new SaveManagerBuilder()
                .UseJsonSerializer()
                .UseLocalFileStorage()
                .Build();
            
            // Setup AutoSaveManager (auto-save every 60 seconds for testing)
            _autoSaveManager = new AutoSaveManager(_saveManager, intervalSeconds: 60f);
            _autoSaveManager.OnAutoSaveComplete += OnAutoSaveComplete;
            
            // Setup BackupManager (keep last 3 backups)
            _backupManager = new BackupManager(_saveManager, maxBackupCount: 3);
            _backupManager.OnBackupCreated += OnBackupCreated;
            _backupManager.OnBackupRestored += OnBackupRestored;
            
            // Initialize player data
            _playerData = new PlayerData
            {
                Name = "Hero",
                Level = 1,
                Gold = 0
            };
            
            // Enable auto-save
            _autoSaveManager.EnableAutoSave("main_profile", _playerData);
            
            SaveSystemLogger.Log("Advanced features example started!");
            SaveSystemLogger.Log($"Auto-save enabled with {_autoSaveManager.AutoSaveInterval}s interval");
        }
        
        private void Update()
        {
            // Update auto-save timer
            _autoSaveManager.Update(Time.deltaTime);
        }
        
        /// <summary>
        /// Manually save and create backup.
        /// </summary>
        public async void SaveWithBackup()
        {
            SaveSystemLogger.Log("Saving with backup...");
            
            // Save
            var saveResult = await _saveManager.SaveAsync("main_profile", _playerData);
            
            if (saveResult.Success)
            {
                SaveSystemLogger.Log("Save successful!");
                
                // Create backup
                var backupResult = await _backupManager.CreateBackupAsync("main_profile");
                
                if (backupResult.Success)
                {
                    SaveSystemLogger.Log($"Backup created at {backupResult.Data.Timestamp}");
                }
            }
        }
        
        /// <summary>
        /// List all backups.
        /// </summary>
        public void ListBackups()
        {
            List<BackupInfo> backups = _backupManager.GetBackups("main_profile");
            
            SaveSystemLogger.Log($"Found {backups.Count} backups:");
            
            for (int i = 0; i < backups.Count; i++)
            {
                var backup = backups[i];
                SaveSystemLogger.Log($"  [{i}] {backup.Timestamp:yyyy-MM-dd HH:mm:ss} - {backup.FileSize} bytes");
            }
        }
        
        /// <summary>
        /// Restore from most recent backup.
        /// </summary>
        public async void RestoreLatestBackup()
        {
            SaveSystemLogger.Log("Restoring from latest backup...");
            
            var result = await _backupManager.RestoreFromBackupAsync("main_profile", backupIndex: 0);
            
            if (result.Success)
            {
                SaveSystemLogger.Log("Backup restored successfully!");
                
                // Reload data
                var loadResult = await _saveManager.LoadAsync<PlayerData>("main_profile");
                if (loadResult.Success)
                {
                    _playerData = loadResult.Data;
                    SaveSystemLogger.Log($"Loaded: {_playerData.Name}, Level {_playerData.Level}");
                }
            }
        }
        
        /// <summary>
        /// Simulate data changes (for testing auto-save).
        /// </summary>
        public void SimulateGameProgress()
        {
            _playerData.Level++;
            _playerData.Gold += 100;
            _playerData.Experience += 50f;
            
            SaveSystemLogger.Log($"Player progress: Level {_playerData.Level}, Gold {_playerData.Gold}");
        }
        
        private void OnAutoSaveComplete(SaveResult result)
        {
            if (result.Success)
            {
                SaveSystemLogger.Log($"✅ Auto-save completed at {_autoSaveManager.LastAutoSave:HH:mm:ss}");
            }
            else
            {
                SaveSystemLogger.LogError($"❌ Auto-save failed: {result.ErrorMessage}");
            }
        }
        
        private void OnBackupCreated(BackupInfo backup)
        {
            SaveSystemLogger.Log($"📦 Backup created: {backup.Timestamp:HH:mm:ss}");
        }
        
        private void OnBackupRestored(BackupInfo backup)
        {
            SaveSystemLogger.Log($"♻️ Backup restored from: {backup.Timestamp:HH:mm:ss}");
        }
        
        private void OnDestroy()
        {
            if (_autoSaveManager != null)
            {
                _autoSaveManager.OnAutoSaveComplete -= OnAutoSaveComplete;
                _autoSaveManager.Cleanup(); // Cleanup to prevent Unity freeze
            }
            
            if (_backupManager != null)
            {
                _backupManager.OnBackupCreated -= OnBackupCreated;
                _backupManager.OnBackupRestored -= OnBackupRestored;
            }
        }
    }
}

