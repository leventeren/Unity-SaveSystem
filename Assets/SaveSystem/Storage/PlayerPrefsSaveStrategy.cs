using SaveSystem.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem.Storage
{
    /// <summary>
    /// PlayerPrefs storage strategy.
    /// Good for simple, small data. Not recommended for large save files.
    /// </summary>
    public class PlayerPrefsSaveStrategy : BaseSaveStrategy
    {
        private const string KeysListKey = "_SaveSystem_Keys";
        private const string KeySeparator = ";";
        
        public PlayerPrefsSaveStrategy() : base(null)
        {
        }
        
        protected override string GetDefaultBasePath()
        {
            return "PlayerPrefs"; // Conceptual, PlayerPrefs doesn't use paths
        }
        
        private string GetPlayerPrefsKey(string key)
        {
            ValidateKey(key);
            return "SaveSystem_" + key;
        }
        
        public override Task SaveAsync(string key, string data, IProgress<float> progress = null)
        {
            ReportProgress(progress, 0f);
            
            string prefsKey = GetPlayerPrefsKey(key);
            PlayerPrefs.SetString(prefsKey, data);
            
            // Track this key in our keys list
            AddKeyToList(key);
            
            ReportProgress(progress, 0.8f);
            
            PlayerPrefs.Save();
            
            ReportProgress(progress, 1f);
            
            return Task.CompletedTask;
        }
        
        public override Task<string> LoadAsync(string key, IProgress<float> progress = null)
        {
            ReportProgress(progress, 0f);
            
            string prefsKey = GetPlayerPrefsKey(key);
            
            if (!PlayerPrefs.HasKey(prefsKey))
            {
                throw new InvalidOperationException($"Save data not found for key: {key}");
            }
            
            string data = PlayerPrefs.GetString(prefsKey);
            
            ReportProgress(progress, 1f);
            
            return Task.FromResult(data);
        }
        
        public override Task DeleteAsync(string key)
        {
            string prefsKey = GetPlayerPrefsKey(key);
            PlayerPrefs.DeleteKey(prefsKey);
            
            // Remove from keys list
            RemoveKeyFromList(key);
            
            PlayerPrefs.Save();
            
            return Task.CompletedTask;
        }
        
        public override Task<bool> ExistsAsync(string key)
        {
            string prefsKey = GetPlayerPrefsKey(key);
            return Task.FromResult(PlayerPrefs.HasKey(prefsKey));
        }
        
        public override Task<string[]> GetAllKeysAsync()
        {
            if (!PlayerPrefs.HasKey(KeysListKey))
            {
                return Task.FromResult(new string[0]);
            }
            
            string keysList = PlayerPrefs.GetString(KeysListKey);
            string[] keys = keysList.Split(new[] { KeySeparator }, StringSplitOptions.RemoveEmptyEntries);
            
            return Task.FromResult(keys);
        }
        
        private void AddKeyToList(string key)
        {
            var existingKeys = GetAllKeysAsync().Result.ToList();
            
            if (!existingKeys.Contains(key))
            {
                existingKeys.Add(key);
                string keysList = string.Join(KeySeparator, existingKeys);
                PlayerPrefs.SetString(KeysListKey, keysList);
            }
        }
        
        private void RemoveKeyFromList(string key)
        {
            var existingKeys = GetAllKeysAsync().Result.ToList();
            
            if (existingKeys.Contains(key))
            {
                existingKeys.Remove(key);
                string keysList = string.Join(KeySeparator, existingKeys);
                PlayerPrefs.SetString(KeysListKey, keysList);
            }
        }
    }
}
