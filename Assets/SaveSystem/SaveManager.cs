using SaveSystem.Core;
using SaveSystem.Models;
using SaveSystem.Features.Backup;
using SaveSystem.Features.AutoSave;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SaveSystem
{
    /// <summary>
    /// Main facade for the save system.
    /// Provides a simple API for save/load operations with all advanced features.
    /// </summary>
    public class SaveManager
    {
        private readonly ISerializer _serializer;
        private readonly ISaveStrategy _saveStrategy;
        private readonly BackupManager _backupManager;  // Optional
        private readonly AutoSaveManager _autoSaveManager;  // Optional
        
        // Events
        public event Action<SaveResult> OnSaveComplete;
        public event Action<SaveResult> OnLoadComplete;
        
        // Properties
        public BackupManager BackupManager => _backupManager;
        public AutoSaveManager AutoSaveManager => _autoSaveManager;
        
        public SaveManager(
            ISerializer serializer, 
            ISaveStrategy saveStrategy,
            BackupManager backupManager = null,
            AutoSaveManager autoSaveManager = null)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _saveStrategy = saveStrategy ?? throw new ArgumentNullException(nameof(saveStrategy));
            _backupManager = backupManager;
            _autoSaveManager = autoSaveManager;
        }
        
        /// <summary>
        /// Saves data asynchronously.
        /// </summary>
        public async Task<SaveResult> SaveAsync<T>(string profileName, T data, IProgress<float> progress = null) where T : ISaveData
        {
            try
            {
                // Auto-enable AutoSave if configured but not yet enabled
                if (_autoSaveManager != null && !_autoSaveManager.IsEnabled)
                {
                    _autoSaveManager.EnableAutoSave(profileName, data);
                }
                
                // Progress: 0-30% Serialization
                progress?.Report(0f);
                string serializedData = _serializer.Serialize(data);
                progress?.Report(0.3f);
                
                // Progress: 30-100% Storage
                var storageProgress = new Progress<float>(p => progress?.Report(0.3f + (p * 0.7f)));
                await _saveStrategy.SaveAsync(profileName, serializedData, storageProgress);
                
                progress?.Report(1f);
                
                var result = SaveResult.Ok();
                OnSaveComplete?.Invoke(result);
                
                // Auto-create backup if BackupManager is configured
                if (_backupManager != null)
                {
                    _ = _backupManager.CreateBackupAsync(profileName); // Fire and forget
                }
                
                return result;
            }
            catch (Exception ex)
            {
                var result = SaveResult.Fail($"Save failed: {ex.Message}", SaveErrorCode.Unknown);
                OnSaveComplete?.Invoke(result);
                return result;
            }
        }
        
        /// <summary>
        /// Loads data asynchronously.
        /// </summary>
        public async Task<SaveResult<T>> LoadAsync<T>(string profileName, IProgress<float> progress = null) where T : ISaveData
        {
            try
            {
                // Progress: 0-70% Storage
                progress?.Report(0f);
                var storageProgress = new Progress<float>(p => progress?.Report(p * 0.7f));
                string serializedData = await _saveStrategy.LoadAsync(profileName, storageProgress);
                progress?.Report(0.7f);
                
                // Progress: 70-100% Deserialization
                T data = _serializer.Deserialize<T>(serializedData);
                progress?.Report(1f);
                
                var result = SaveResult<T>.Ok(data);
                OnLoadComplete?.Invoke(SaveResult.Ok());
                return result;
            }
            catch (FileNotFoundException)
            {
                var result = SaveResult<T>.Fail($"Save file not found: {profileName}", SaveErrorCode.FileNotFound);
                OnLoadComplete?.Invoke(SaveResult.Fail(result.ErrorMessage, result.ErrorCode));
                return result;
            }
            catch (Exception ex)
            {
                var result = SaveResult<T>.Fail($"Load failed: {ex.Message}", SaveErrorCode.Unknown);
                OnLoadComplete?.Invoke(SaveResult.Fail(result.ErrorMessage, result.ErrorCode));
                return result;
            }
        }
        
        /// <summary>
        /// Deletes a save profile.
        /// </summary>
        public async Task<SaveResult> DeleteAsync(string profileName)
        {
            try
            {
                await _saveStrategy.DeleteAsync(profileName);
                return SaveResult.Ok();
            }
            catch (Exception ex)
            {
                return SaveResult.Fail($"Delete failed: {ex.Message}", SaveErrorCode.Unknown);
            }
        }
        
        /// <summary>
        /// Checks if a save profile exists.
        /// </summary>
        public async Task<bool> ExistsAsync(string profileName)
        {
            return await _saveStrategy.ExistsAsync(profileName);
        }
        
        /// <summary>
        /// Gets all save profile names.
        /// </summary>
        public async Task<string[]> GetAllProfilesAsync()
        {
            return await _saveStrategy.GetAllKeysAsync();
        }
    }
}
