using SaveSystem;
using SaveSystem.Features.AutoSave;
using SaveSystem.Features.Backup;
using SaveSystem.Examples;
using UnityEngine;
using SaveSystem.Utilities;
using Zenject;

namespace SaveSystem.Examples
{
    /// <summary>
    /// Example showing SaveSystem with Zenject dependency injection.
    /// All dependencies are injected via constructor.
    /// </summary>
    public class ZenjectSaveExample : MonoBehaviour
    {
        // Dependencies injected by Zenject
        private readonly SaveManager _saveManager;
        private readonly AutoSaveManager _autoSaveManager;
        private readonly BackupManager _backupManager;
        
        private PlayerData _playerData;
        
        // Constructor injection - Zenject will automatically inject these
        [Inject]
        public ZenjectSaveExample(
            SaveManager saveManager,
            AutoSaveManager autoSaveManager,
            BackupManager backupManager)
        {
            _saveManager = saveManager;
            _autoSaveManager = autoSaveManager;
            _backupManager = backupManager;
            
            SaveSystemLogger.Log("✅ SaveSystem dependencies injected via Zenject!");
        }
        
        private void Start()
        {
            // Initialize player data
            _playerData = new PlayerData
            {
                Name = "Zenject Hero",
                Level = 1,
                Gold = 500
            };
            
            // Enable auto-save
            _autoSaveManager.EnableAutoSave("zenject_profile", _playerData);
            
            SaveSystemLogger.Log("Zenject example started with auto-save enabled!");
        }
        
        private void Update()
        {
            // Update auto-save timer
            _autoSaveManager?.Update(Time.deltaTime);
        }
        
        /// <summary>
        /// Save using injected SaveManager.
        /// </summary>
        public async void SaveGame()
        {
            SaveSystemLogger.Log("Saving via Zenject-injected SaveManager...");
            
            var result = await _saveManager.SaveAsync("zenject_profile", _playerData);
            
            if (result.Success)
            {
                SaveSystemLogger.Log("✅ Game saved successfully!");
                
                // Also create backup
                await _backupManager.CreateBackupAsync("zenject_profile");
            }
            else
            {
                SaveSystemLogger.LogError($"❌ Save failed: {result.ErrorMessage}");
            }
        }
        
        /// <summary>
        /// Load using injected SaveManager.
        /// </summary>
        public async void LoadGame()
        {
            SaveSystemLogger.Log("Loading via Zenject-injected SaveManager...");
            
            var result = await _saveManager.LoadAsync<PlayerData>("zenject_profile");
            
            if (result.Success)
            {
                _playerData = result.Data;
                SaveSystemLogger.Log($"✅ Loaded: {_playerData.Name}, Level {_playerData.Level}, Gold {_playerData.Gold}");
            }
            else
            {
                SaveSystemLogger.LogError($"❌ Load failed: {result.ErrorMessage}");
            }
        }
        
        /// <summary>
        /// Simulate game progress (for testing auto-save).
        /// </summary>
        public void PlayGame()
        {
            _playerData.Level++;
            _playerData.Gold += 100;
            
            SaveSystemLogger.Log($"📈 Progress: Level {_playerData.Level}, Gold {_playerData.Gold}");
        }
    }
}

