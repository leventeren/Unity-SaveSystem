using SaveSystem.Core;
using System;
using System.Threading.Tasks;

namespace SaveSystem.Storage
{
    /// <summary>
    /// Abstract base class for save strategies.
    /// Provides common functionality and template methods.
    /// </summary>
    public abstract class BaseSaveStrategy : ISaveStrategy
    {
        protected string BasePath { get; set; }
        
        public BaseSaveStrategy(string basePath = null)
        {
            BasePath = basePath ?? GetDefaultBasePath();
        }
        
        public abstract Task SaveAsync(string key, string data, IProgress<float> progress = null);
        public abstract Task<string> LoadAsync(string key, IProgress<float> progress = null);
        public abstract Task DeleteAsync(string key);
        public abstract Task<bool> ExistsAsync(string key);
        public abstract Task<string[]> GetAllKeysAsync();
        
        /// <summary>
        /// Gets the default base path for this platform.
        /// </summary>
        protected abstract string GetDefaultBasePath();
        
        /// <summary>
        /// Validates a save key.
        /// </summary>
        protected virtual bool ValidateKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Save key cannot be null or empty.", nameof(key));
            }
            
            // Check for invalid characters
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            if (key.IndexOfAny(invalidChars) >= 0)
            {
                throw new ArgumentException($"Save key contains invalid characters: {key}", nameof(key));
            }
            
            return true;
        }
        
        /// <summary>
        /// Reports progress to the progress reporter.
        /// </summary>
        protected void ReportProgress(IProgress<float> progress, float value)
        {
            progress?.Report(value);
        }
    }
}
