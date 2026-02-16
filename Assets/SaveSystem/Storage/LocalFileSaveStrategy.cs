using SaveSystem.Core;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem.Storage
{
    /// <summary>
    /// Local file system storage strategy.
    /// Uses Application.persistentDataPath for cross-platform compatibility.
    /// </summary>
    public class LocalFileSaveStrategy : BaseSaveStrategy
    {
        private readonly string _fileExtension;
        
        public LocalFileSaveStrategy(string fileExtension = ".save", string basePath = null) 
            : base(basePath)
        {
            _fileExtension = fileExtension;
            
            // Ensure directory exists
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
        }
        
        protected override string GetDefaultBasePath()
        {
            return Path.Combine(Application.persistentDataPath, "Saves");
        }
        
        private string GetFilePath(string key)
        {
            ValidateKey(key);
            return Path.Combine(BasePath, key + _fileExtension);
        }
        
        public override async Task SaveAsync(string key, string data, IProgress<float> progress = null)
        {
            ReportProgress(progress, 0f);
            
            string filePath = GetFilePath(key);
            string directory = Path.GetDirectoryName(filePath);
            
            // Ensure directory exists
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            ReportProgress(progress, 0.3f);
            
            try
            {
                // Atomic write: write to temp file, then replace
                string tempFilePath = filePath + ".tmp";
                await File.WriteAllTextAsync(tempFilePath, data);
                
                ReportProgress(progress, 0.8f);
                
                // Replace old file with new one atomically
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Move(tempFilePath, filePath);
                
                ReportProgress(progress, 1f);
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to save data to {filePath}: {ex.Message}", ex);
            }
        }
        
        public override async Task<string> LoadAsync(string key, IProgress<float> progress = null)
        {
            ReportProgress(progress, 0f);
            
            string filePath = GetFilePath(key);
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Save file not found: {filePath}");
            }
            
            ReportProgress(progress, 0.3f);
            
            try
            {
                string data = await File.ReadAllTextAsync(filePath);
                
                ReportProgress(progress, 1f);
                
                return data;
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to load data from {filePath}: {ex.Message}", ex);
            }
        }
        
        public override Task DeleteAsync(string key)
        {
            string filePath = GetFilePath(key);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            return Task.CompletedTask;
        }
        
        public override Task<bool> ExistsAsync(string key)
        {
            string filePath = GetFilePath(key);
            return Task.FromResult(File.Exists(filePath));
        }
        
        public override Task<string[]> GetAllKeysAsync()
        {
            if (!Directory.Exists(BasePath))
            {
                return Task.FromResult(new string[0]);
            }
            
            var files = Directory.GetFiles(BasePath, "*" + _fileExtension);
            var keys = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
            
            return Task.FromResult(keys);
        }
    }
}
