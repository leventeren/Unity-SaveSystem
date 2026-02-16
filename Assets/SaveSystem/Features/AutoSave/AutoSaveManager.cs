using SaveSystem.Core;
using SaveSystem.Models;
using SaveSystem.Utilities;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem.Features.AutoSave
{
    /// <summary>
    /// Auto-save manager with configurable intervals.
    /// Automatically saves data at specified intervals or on specific events.
    /// </summary>
    public class AutoSaveManager
    {
        private readonly SaveManager _saveManager;
        private float _autoSaveInterval = 300f; // 5 minutes default
        private float _timeSinceLastSave = 0f;
        private bool _isEnabled = false;
        private string _activeProfileName;
        private ISaveData _activeData;
        
        public bool IsEnabled => _isEnabled;
        public float AutoSaveInterval => _autoSaveInterval;
        public DateTime LastAutoSave { get; private set; }
        
        // Events
        public event Action<SaveResult> OnAutoSaveComplete;
        
        public AutoSaveManager(SaveManager saveManager, float intervalSeconds = 300f)
        {
            _saveManager = saveManager ?? throw new ArgumentNullException(nameof(saveManager));
            _autoSaveInterval = intervalSeconds;
        }
        
        /// <summary>
        /// Enables auto-save for the specified profile.
        /// </summary>
        public void EnableAutoSave<T>(string profileName, T data) where T : ISaveData
        {
            _activeProfileName = profileName;
            _activeData = data;
            _isEnabled = true;
            _timeSinceLastSave = 0f;
            
            SaveSystemLogger.Log($"Auto-save enabled for profile '{profileName}' with interval {_autoSaveInterval}s");
        }
        
        /// <summary>
        /// Disables auto-save.
        /// </summary>
        public void DisableAutoSave()
        {
            _isEnabled = false;
            _activeProfileName = null;
            _activeData = null;
            
            SaveSystemLogger.Log("Auto-save disabled");
        }
        
        /// <summary>
        /// Updates auto-save timer. Call this in Update() or similar.
        /// </summary>
        public void Update(float deltaTime)
        {
            if (!_isEnabled || _activeData == null)
                return;
            
            _timeSinceLastSave += deltaTime;
            
            if (_timeSinceLastSave >= _autoSaveInterval)
            {
                TriggerAutoSaveAsync().ConfigureAwait(false);
            }
        }
        
        /// <summary>
        /// Manually triggers an auto-save.
        /// </summary>
        public async Task TriggerAutoSaveAsync()
        {
            if (_activeData == null || string.IsNullOrEmpty(_activeProfileName))
            {
                Debug.LogWarning("Auto-save triggered but no active profile/data set.");
                return;
            }
            
            _timeSinceLastSave = 0f;
            
            Debug.Log($"Auto-saving profile '{_activeProfileName}'...");
            
            var result = await _saveManager.SaveAsync(_activeProfileName, _activeData);
            
            if (result.Success)
            {
                LastAutoSave = DateTime.UtcNow;
                Debug.Log($"Auto-save successful at {LastAutoSave:HH:mm:ss}");
            }
            else
            {
                Debug.LogError($"Auto-save failed: {result.ErrorMessage}");
            }
            
            OnAutoSaveComplete?.Invoke(result);
        }
        
        /// <summary>
        /// Call this when disabling the MonoBehaviour to cleanup resources.
        /// Should be called from OnDestroy or OnDisable.
        /// </summary>
        public void Cleanup()
        {
            DisableAutoSave();
        }
    }
}
