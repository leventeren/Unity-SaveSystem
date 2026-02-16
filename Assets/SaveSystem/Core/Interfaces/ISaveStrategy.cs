using System;
using System.Threading.Tasks;

namespace SaveSystem.Core
{
    /// <summary>
    /// Strategy interface for storage implementations.
    /// Supports different storage mechanisms (File, PlayerPrefs, Cloud).
    /// </summary>
    public interface ISaveStrategy
    {
        /// <summary>
        /// Saves data asynchronously.
        /// </summary>
        /// <param name="key">Unique identifier for the save.</param>
        /// <param name="data">Serialized data to save.</param>
        /// <param name="progress">Optional progress reporter (0-1).</param>
        Task SaveAsync(string key, string data, IProgress<float> progress = null);
        
        /// <summary>
        /// Loads data asynchronously.
        /// </summary>
        /// <param name="key">Unique identifier for the save.</param>
        /// <param name="progress">Optional progress reporter (0-1).</param>
        /// <returns>Serialized data string.</returns>
        Task<string> LoadAsync(string key, IProgress<float> progress = null);
        
        /// <summary>
        /// Deletes save data asynchronously.
        /// </summary>
        /// <param name="key">Unique identifier for the save.</param>
        Task DeleteAsync(string key);
        
        /// <summary>
        /// Checks if save data exists.
        /// </summary>
        /// <param name="key">Unique identifier for the save.</param>
        /// <returns>True if save exists, false otherwise.</returns>
        Task<bool> ExistsAsync(string key);
        
        /// <summary>
        /// Gets all save keys.
        /// </summary>
        /// <returns>Array of save keys.</returns>
        Task<string[]> GetAllKeysAsync();
    }
}
