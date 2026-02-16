using SaveSystem.Models;
using SaveSystem.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem.Features.Backup
{
    /// <summary>
    /// Backup manager for save files.
    /// Creates and manages backup copies of save files.
    /// </summary>
    public class BackupManager
    {
        private readonly SaveManager _saveManager;
        private readonly string _backupBasePath;
        private readonly string _savesBasePath;  // Cache for thread safety
        private readonly int _maxBackupCount;
        
        public event Action<BackupInfo> OnBackupCreated;
        public event Action<BackupInfo> OnBackupRestored;
        
        public BackupManager(SaveManager saveManager, int maxBackupCount = 3, string backupBasePath = null)
        {
            _saveManager = saveManager ?? throw new ArgumentNullException(nameof(saveManager));
            _maxBackupCount = maxBackupCount;
            
            // Cache paths on main thread for thread-safe access
            _backupBasePath = backupBasePath ?? Path.Combine(Application.persistentDataPath, "Backups");
            _savesBasePath = Path.Combine(Application.persistentDataPath, "Saves");
            
            // Ensure backup directory exists
            if (!Directory.Exists(_backupBasePath))
            {
                Directory.CreateDirectory(_backupBasePath);
            }
        }
        
        /// <summary>
        /// Creates a backup of the specified save profile.
        /// </summary>
        public async Task<SaveResult<BackupInfo>> CreateBackupAsync(string profileName)
        {
            // Run file operations on background thread to avoid blocking
            return await Task.Run(() =>
            {
                try
                {
                    // Get original save file path
                    string savePath = GetSaveFilePath(profileName);
                    
                    if (!File.Exists(savePath))
                    {
                        SaveSystemLogger.LogError($"Backup failed: Save file not found at '{savePath}'");
                        return SaveResult<BackupInfo>.Fail($"Save file not found: {profileName}", SaveErrorCode.FileNotFound);
                    }
                    
                    // Create backup directory for this profile
                    string profileBackupDir = Path.Combine(_backupBasePath, profileName);
                    if (!Directory.Exists(profileBackupDir))
                    {
                        Directory.CreateDirectory(profileBackupDir);
                    }
                    
                    // Manage backup rotation (keep only maxBackupCount)
                    var existingBackups = GetBackups(profileName).OrderByDescending(b => b.Timestamp).ToList();
                    
                    while (existingBackups.Count >= _maxBackupCount)
                    {
                        // Delete oldest backup
                        var oldest = existingBackups.Last();
                        File.Delete(oldest.FilePath);
                        existingBackups.RemoveAt(existingBackups.Count - 1);
                    }
                    
                    // Create new backup
                    string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                    string backupFileName = $"{profileName}_backup_{timestamp}.bak";
                    string backupPath = Path.Combine(profileBackupDir, backupFileName);
                    
                    // Copy file
                    File.Copy(savePath, backupPath, overwrite: true);
                    
                    var backupInfo = new BackupInfo
                    {
                        ProfileName = profileName,
                        Timestamp = DateTime.UtcNow,
                        FilePath = backupPath,
                        FileSize = new FileInfo(backupPath).Length
                    };
                    
                    OnBackupCreated?.Invoke(backupInfo);
                    SaveSystemLogger.LogSuccess($"Backup created: {backupFileName}");
                    
                    return SaveResult<BackupInfo>.Ok(backupInfo);
                }
                catch (Exception ex)
                {
                    return SaveResult<BackupInfo>.Fail($"Backup creation failed: {ex.Message}", SaveErrorCode.Unknown);
                }
            });
        }
        
        /// <summary>
        /// Restores a backup. Index 0 is the most recent.
        /// </summary>
        public async Task<SaveResult> RestoreFromBackupAsync(string profileName, int backupIndex = 0)
        {
            // Run file operations on background thread to avoid blocking
            return await Task.Run(() =>
            {
                try
                {
                    var backups = GetBackups(profileName).OrderByDescending(b => b.Timestamp).ToList();
                    
                    if (backupIndex < 0 || backupIndex >= backups.Count)
                    {
                        return SaveResult.Fail($"Invalid backup index: {backupIndex}", SaveErrorCode.FileNotFound);
                    }
                    
                    var backup = backups[backupIndex];
                    string savePath = GetSaveFilePath(profileName);
                    
                    // Copy backup to save location
                    File.Copy(backup.FilePath, savePath, overwrite: true);
                    
                    OnBackupRestored?.Invoke(backup);
                    Debug.Log($"Backup restored from {backup.Timestamp:yyyy-MM-dd HH:mm:ss}");
                    
                    return SaveResult.Ok();
                }
                catch (Exception ex)
                {
                    return SaveResult.Fail($"Backup restore failed: {ex.Message}", SaveErrorCode.Unknown);
                }
            });
        }
        
        /// <summary>
        /// Gets all backups for a profile.
        /// </summary>
        public List<BackupInfo> GetBackups(string profileName)
        {
            var backups = new List<BackupInfo>();
            string profileBackupDir = Path.Combine(_backupBasePath, profileName);
            
            if (!Directory.Exists(profileBackupDir))
                return backups;
            
            var files = Directory.GetFiles(profileBackupDir, "*.bak");
            
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                backups.Add(new BackupInfo
                {
                    ProfileName = profileName,
                    Timestamp = fileInfo.LastWriteTimeUtc,
                    FilePath = file,
                    FileSize = fileInfo.Length
                });
            }
            
            return backups;
        }
        
        private string GetSaveFilePath(string profileName)
        {
            // Use cached path - thread safe!
            return Path.Combine(_savesBasePath, profileName + ".json");
        }
    }
    
    /// <summary>
    /// Backup metadata information.
    /// </summary>
    [Serializable]
    public class BackupInfo
    {
        public string ProfileName { get; set; }
        public DateTime Timestamp { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
    }
}
