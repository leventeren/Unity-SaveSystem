using System.Threading.Tasks;
using SaveSystem.Utilities;
using SaveSystem;
using SaveSystem.Examples;
using TMPro;
using UnityEngine;

namespace SaveSystem.Examples
{
    /// <summary>
    /// Simple cross-platform SaveSystem example.
    /// Uses auto-detection for optimal platform settings.
    /// </summary>
    public class CrossPlatformExample : MonoBehaviour
    {
        [Header("UI Settings")]
        [SerializeField] private TextMeshProUGUI playerLevelText;
        [SerializeField] private TextMeshProUGUI playerGoldText;

        [Header("Test Options")]
        [SerializeField] private bool testOnStart = true;
        [SerializeField] private bool autoSaveAndLoad = true;
        
        private SaveManager _saveManager;
        private PlayerData _playerData;
        
        private async void Start()
        {
            // ✨ AUTO-DETECT PLATFORM - One line setup!
            _saveManager = new SaveManagerBuilder()
                .UseAutoDetectPlatform()  // 🎯 Automatically chooses best settings
                .Build();
            
            LogPlatformInfo();
            
            // Check if save exists and load it
            await InitializeOrLoadData();
            
            // Auto-test if enabled
            if (testOnStart)
            {
                TestSaveSystem();
            }
        }
        
        /// <summary>
        /// Check if save exists - load it, otherwise create new data.
        /// </summary>
        private async Task InitializeOrLoadData()
        {
            bool saveExists = await _saveManager.ExistsAsync("my_save");
            
            if (saveExists)
            {
                SaveSystemLogger.Log("💾 Save file found! Loading...");
                
                var result = await _saveManager.LoadAsync<PlayerData>("my_save");
                
                if (result.Success)
                {
                    _playerData = result.Data;
                    SaveSystemLogger.Log($"✅ Loaded existing save: Level {_playerData.Level}, Gold {_playerData.Gold}");
                }
                else
                {
                    SaveSystemLogger.LogWarning($"⚠️ Failed to load save: {result.ErrorMessage}");
                    CreateNewPlayerData();
                }
            }
            else
            {
                SaveSystemLogger.Log("🆕 No save found, creating new player data");
                CreateNewPlayerData();
            }
            
            // Update UI with loaded/created data
            UpdateUI();
        }
        
        /// <summary>
        /// Create new player data with default values.
        /// </summary>
        private void CreateNewPlayerData()
        {
            _playerData = new PlayerData
            {
                Name = "Player",
                Level = 1,
                Gold = 100
            };
        }
        
        /// <summary>
        /// Automatic test - saves and loads to verify system works.
        /// </summary>
        private async void TestSaveSystem()
        {
            SaveSystemLogger.Log("🧪 Testing SaveSystem...");
            
            // Test data
            _playerData.Level = 5;
            _playerData.Gold = 250;
            
            // Save
            await SaveGameAsync();  // Use internal async Task method
            
            // Wait a frame
            await Task.Yield();
            
            // Load
            await LoadGameAsync();  // Use internal async Task method
            
            SaveSystemLogger.Log("✅ SaveSystem test complete!");
        }
        
        /// <summary>
        /// Save game (works on all platforms).
        /// Right-click on component → Save Game
        /// </summary>
        [ContextMenu("Save Game")]
        public async void SaveGame()
        {
            await SaveGameAsync();
        }

        private async Task SaveGameAsync()
        {
            SaveSystemLogger.Log("Saving game...");
            
            var result = await _saveManager.SaveAsync("my_save", _playerData);
            
            if (result.Success)
            {
                SaveSystemLogger.Log("✅ Game saved successfully!");
            }
            else
            {
                SaveSystemLogger.LogError($"❌ Save failed: {result.ErrorMessage}");
            }
            
            UpdateUI();
        }
        
        /// <summary>
        /// Load game (works on all platforms).
        /// Right-click on component → Load Game
        /// </summary>
        [ContextMenu("Load Game")]
        public async void LoadGame()
        {
            await LoadGameAsync();
        }
        
        private async Task LoadGameAsync()
        {
            SaveSystemLogger.Log("Loading game...");
            
            var result = await _saveManager.LoadAsync<PlayerData>("my_save");
            
            if (result.Success)
            {
                _playerData = result.Data;
                SaveSystemLogger.Log($"✅ Game loaded: {_playerData.Name}, Level {_playerData.Level}");
            }
            else
            {
                SaveSystemLogger.LogWarning($"⚠️ Load failed: {result.ErrorMessage}");
            }
            
            UpdateUI();
        }
        
        /// <summary>
        /// Test game progress.
        /// Right-click on component → Test Progress
        /// </summary>
        [ContextMenu("Test Progress")]
        public void TestProgress()
        {
            _playerData.Level++;
            _playerData.Gold += 50;
            SaveSystemLogger.Log($"Progress: Level {_playerData.Level}, Gold {_playerData.Gold}");
            
            UpdateUI();
        }
        
        /// <summary>
        /// Delete save file.
        /// Right-click on component → Delete Save
        /// </summary>
        [ContextMenu("Delete Save")]
        public async void DeleteSave()
        {
            await DeleteSaveAsync();
        }
        
        private async Task DeleteSaveAsync()
        {
            SaveSystemLogger.Log("Deleting save...");
            
            var result = await _saveManager.DeleteAsync("my_save");
            
            if (result.Success)
            {
                SaveSystemLogger.Log("✅ Save deleted!");
                CreateNewPlayerData();
                UpdateUI();
            }
            else
            {
                SaveSystemLogger.LogError($"❌ Delete failed: {result.ErrorMessage}");
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
        
        private void LogPlatformInfo()
        {
            SaveSystemLogger.Log("=== Cross-Platform SaveSystem ===");
            SaveSystemLogger.Log($"Platform: {Application.platform}");
            SaveSystemLogger.Log($"Save Path: {Application.persistentDataPath}");
            
#if UNITY_ANDROID || UNITY_IOS
            SaveSystemLogger.Log("Storage: PlayerPrefs (Mobile Optimized)");
#else
            SaveSystemLogger.Log("Storage: LocalFile (PC Optimized)");
#endif
            
            SaveSystemLogger.Log("=================================");
        }
    }
}

