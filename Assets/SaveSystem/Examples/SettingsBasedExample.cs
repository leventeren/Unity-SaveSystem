using SaveSystem;
using SaveSystem.Examples;
using SaveSystem.Utilities;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SaveSystem.Examples
{
    /// <summary>
    /// Cross-platform SaveSystem example with ScriptableObject settings.
    /// Demonstrates using SaveSystemSettings for easy configuration.
    /// </summary>
    public class SettingsBasedExample : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private SaveSystemSettings saveSettings;
        
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI playerLevelText;
        [SerializeField] private TextMeshProUGUI playerGoldText;
        [SerializeField] private TextMeshProUGUI statusText;
        
        private SaveManager _saveManager;
        private PlayerData _playerData;
        
        private async void Start()
        {
            // ✨ ONE-LINE SETUP - Settings automatically configures everything!
            _saveManager = new SaveManagerBuilder()
                .UseSettings(saveSettings)
                .Build();
            
            LogSettingsInfo();
            
            // Check if save exists and load it
            await InitializeOrLoadData();
            
            UpdateUI();
            // AutoSaveManager automatically enabled on first save!
        }
        
        /// <summary>
        /// Check if save exists - load it, otherwise create new data.
        /// </summary>
        private async Task InitializeOrLoadData()
        {
            string profileName = saveSettings?.defaultProfileName ?? "Player";
            bool saveExists = await _saveManager.ExistsAsync(profileName);
            
            if (saveExists)
            {
                SaveSystemLogger.Log($"💾 Save file found for '{profileName}'! Loading...");
                
                var result = await _saveManager.LoadAsync<PlayerData>(profileName);
                
                if (result.Success)
                {
                    _playerData = result.Data;
                    SaveSystemLogger.Log($"✅ Loaded existing save: Level {_playerData.Level}, Gold {_playerData.Gold}");
                    UpdateStatus($"Loaded: {profileName}");
                }
                else
                {
                    SaveSystemLogger.LogWarning($"⚠️ Failed to load save: {result.ErrorMessage}");
                    CreateNewPlayerData();
                    UpdateStatus("Load failed, created new");
                }
            }
            else
            {
                SaveSystemLogger.Log($"🆕 No save found for '{profileName}', creating new player data");
                CreateNewPlayerData();
                UpdateStatus("New game started");
            }
        }
        
        /// <summary>
        /// Create new player data with default values.
        /// </summary>
        private void CreateNewPlayerData()
        {
            _playerData = new PlayerData
            {
                Name = saveSettings?.defaultProfileName ?? "Player",
                Level = 1,
                Gold = 100
            };
        }
        
        /// <summary>
        /// Save game - can be called from UI button.
        /// </summary>
        [ContextMenu("Save Game")]
        public async void SaveGame()
        {
            await SaveGameAsync();
        }
        
        private async Task SaveGameAsync()
        {
            string profileName = saveSettings?.defaultProfileName ?? "Player";
            SaveSystemLogger.Log($"Saving game to '{profileName}'...");
            UpdateStatus("Saving...");
            
            var result = await _saveManager.SaveAsync(profileName, _playerData);
            
            if (result.Success)
            {
                SaveSystemLogger.Log("✅ Game saved successfully!");
                UpdateStatus("Saved!");
            }
            else
            {
                SaveSystemLogger.LogError($"❌ Save failed: {result.ErrorMessage}");
                UpdateStatus("Save failed!");
            }
            
            UpdateUI();
            // Backup automatically created by SaveManager core!
        }
        
        /// <summary>
        /// Load game - can be called from UI button.
        /// </summary>
        [ContextMenu("Load Game")]
        public async void LoadGame()
        {
            await LoadGameAsync();
        }
        
        private async Task LoadGameAsync()
        {
            string profileName = saveSettings?.defaultProfileName ?? "Player";
            SaveSystemLogger.Log($"Loading game from '{profileName}'...");
            UpdateStatus("Loading...");
            
            var result = await _saveManager.LoadAsync<PlayerData>(profileName);
            
            if (result.Success)
            {
                _playerData = result.Data;
                SaveSystemLogger.Log($"✅ Game loaded: {_playerData.Name}, Level {_playerData.Level}");
                UpdateStatus("Loaded!");
            }
            else
            {
                SaveSystemLogger.LogWarning($"⚠️ Load failed: {result.ErrorMessage}");
                UpdateStatus("Load failed!");
            }
            
            UpdateUI();
            // Backup automatically created by SaveManager core!
        }
        
        /// <summary>
        /// Test game progress - can be called from UI button.
        /// </summary>
        [ContextMenu("Gain Level & Gold")]
        public void GainProgress()
        {
            if (_playerData == null)
            {
                SaveSystemLogger.LogWarning("Player data not initialized!");
                return;
            }
            
            _playerData.Level++;
            _playerData.Gold += 50;
            SaveSystemLogger.Log($"Progress: Level {_playerData.Level}, Gold {_playerData.Gold}");
            UpdateStatus($"Level up! Now {_playerData.Level}");
            
            UpdateUI();
        }
        
        /// <summary>
        /// Delete save file - can be called from UI button.
        /// </summary>
        [ContextMenu("Delete Save")]
        public async void DeleteSave()
        {
            await DeleteSaveAsync();
        }
        
        private async Task DeleteSaveAsync()
        {
            string profileName = saveSettings?.defaultProfileName ?? "Player";
            SaveSystemLogger.Log($"Deleting save '{profileName}'...");
            UpdateStatus("Deleting...");
            
            var result = await _saveManager.DeleteAsync(profileName);
            
            if (result.Success)
            {
                SaveSystemLogger.Log("✅ Save deleted!");
                UpdateStatus("Deleted! Creating new data...");
                CreateNewPlayerData();
                UpdateUI();
            }
            else
            {
                SaveSystemLogger.LogError($"❌ Delete failed: {result.ErrorMessage}");
                UpdateStatus("Delete failed!");
            }
        }
        
        /// <summary>
        /// Updates UI text elements with current player data.
        /// </summary>
        private void UpdateUI()
        {
            if (_playerData == null)
                return;
            
            if (playerLevelText != null)
            {
                playerLevelText.text = $"Level: {_playerData.Level}";
            }
            
            if (playerGoldText != null)
            {
                playerGoldText.text = $"Gold: {_playerData.Gold}";
            }
        }
        
        private void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }
        
        private void LogSettingsInfo()
        {
            if (saveSettings == null)
            {
                SaveSystemLogger.LogWarning("⚠️ SaveSystemSettings not assigned!");
                return;
            }
            
            SaveSystemLogger.Log("=== SaveSystem Settings ===");
            SaveSystemLogger.Log($"Profile: {saveSettings.defaultProfileName}");
            SaveSystemLogger.Log($"Serializer: {saveSettings.serializerType}");
            SaveSystemLogger.Log($"Storage: {saveSettings.storageType}");
            SaveSystemLogger.Log($"Auto-Save: {(saveSettings.enableAutoSave ? $"Enabled ({saveSettings.autoSaveInterval}s)" : "Disabled")}");
            SaveSystemLogger.Log($"Backup: {(saveSettings.enableBackup ? $"Enabled ({saveSettings.maxBackupCount} max)" : "Disabled")}");
            SaveSystemLogger.Log($"Compression: {(saveSettings.useCompression ? "Enabled" : "Disabled")}");
            SaveSystemLogger.Log("===========================");
        }
        
        private void OnDestroy()
        {
            // AutoSaveManager managed by SaveManager core
            // No manual cleanup needed!
        }
    }
}
