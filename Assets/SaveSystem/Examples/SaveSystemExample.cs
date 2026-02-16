using SaveSystem.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using SaveSystem.Utilities;

namespace SaveSystem.Examples
{
    /// <summary>
    /// Example MonoBehaviour demonstrating SaveSystem usage.
    /// Shows basic save/load operations with progress tracking and error handling.
    /// </summary>
    public class SaveSystemExample : MonoBehaviour
    {
        private SaveManager _saveManager;
        private PlayerData _playerData;
        private PlayerInventory _inventory;
        
        private void Start()
        {
            // Setup SaveManager with fluent builder
            _saveManager = new SaveManagerBuilder()
                .UseJsonSerializer()
                .UseLocalFileStorage()
                .Build();
            
            // Subscribe to events
            _saveManager.OnSaveComplete += OnSaveCompleted;
            _saveManager.OnLoadComplete += OnLoadCompleted;
            
            // Initialize default data
            InitializeDefaultData();
        }
        
        private void InitializeDefaultData()
        {
            _playerData = new PlayerData
            {
                Name = "Hero",
                Level = 1,
                Gold = 100,
                Experience = 0f,
                Health = 100,
                MaxHealth = 100
            };
            
            _inventory = new PlayerInventory();
            
            // Add some sample cards
            _inventory.OwnedCards.Add(new Card
            {
                Id = "card_001",
                Name = "Fire Dragon",
                Attack = 100,
                Defense = 50,
                Rarity = "Legendary",
                Abilities = new List<string> { "Fire Breath", "Fly" }
            });
            
            _inventory.CardCounts["Fire Dragon"] = 3;
        }
        
        /// <summary>
        /// Example: Save player data
        /// </summary>
        public async void SavePlayerData()
        {
            SaveSystemLogger.Log("Saving player data...");
            
            var progress = new Progress<float>(p => 
                SaveSystemLogger.Log($"Save progress: {p * 100:F0}%"));
            
            var result = await _saveManager.SaveAsync("player_profile", _playerData, progress);
            
            if (result.Success)
            {
                SaveSystemLogger.Log("Player data saved successfully!");
            }
            else
            {
                SaveSystemLogger.LogError($"Save failed: {result.ErrorMessage}");
            }
        }
        
        /// <summary>
        /// Example: Load player data
        /// </summary>
        public async void LoadPlayerData()
        {
            SaveSystemLogger.Log("Loading player data...");
            
            var progress = new Progress<float>(p => 
                SaveSystemLogger.Log($"Load progress: {p * 100:F0}%"));
            
            var result = await _saveManager.LoadAsync<PlayerData>("player_profile", progress);
            
            if (result.Success)
            {
                _playerData = result.Data;
                SaveSystemLogger.Log($"Player data loaded: {_playerData.Name}, Level {_playerData.Level}");
            }
            else
            {
                SaveSystemLogger.LogError($"Load failed: {result.ErrorMessage}");
            }
        }
        
        /// <summary>
        /// Example: Save inventory
        /// </summary>
        public async void SaveInventory()
        {
            SaveSystemLogger.Log("Saving inventory...");
            
            var result = await _saveManager.SaveAsync("player_inventory", _inventory);
            
            if (result.Success)
            {
                SaveSystemLogger.Log($"Inventory saved! {_inventory.OwnedCards.Count} cards");
            }
        }
        
        /// <summary>
        /// Example: Load inventory
        /// </summary>
        public async void LoadInventory()
        {
            SaveSystemLogger.Log("Loading inventory...");
            
            var result = await _saveManager.LoadAsync<PlayerInventory>("player_inventory");
            
            if (result.Success)
            {
                _inventory = result.Data;
                SaveSystemLogger.Log($"Inventory loaded! {_inventory.OwnedCards.Count} cards");
                
                foreach (var card in _inventory.OwnedCards)
                {
                    SaveSystemLogger.Log($"Card: {card.Name}, ATK: {card.Attack}, DEF: {card.Defense}");
                }
            }
        }
        
        /// <summary>
        /// Example: Delete save
        /// </summary>
        public async void DeleteSave(string profileName)
        {
            var result = await _saveManager.DeleteAsync(profileName);
            
            if (result.Success)
            {
                SaveSystemLogger.Log($"Profile deleted: {profileName}");
            }
        }
        
        /// <summary>
        /// Example: Check if save exists
        /// </summary>
        public async void CheckSaveExists(string profileName)
        {
            bool exists = await _saveManager.ExistsAsync(profileName);
            SaveSystemLogger.Log($"Profile '{profileName}' exists: {exists}");
        }
        
        /// <summary>
        /// Example: List all saves
        /// </summary>
        public async void ListAllSaves()
        {
            string[] profiles = await _saveManager.GetAllProfilesAsync();
            SaveSystemLogger.Log($"Found {profiles.Length} save profiles:");
            
            foreach (string profile in profiles)
            {
                SaveSystemLogger.Log($"- {profile}");
            }
        }
        
        private void OnSaveCompleted(SaveResult result)
        {
            SaveSystemLogger.Log($"Save operation completed. Success: {result.Success}");
        }
        
        private void OnLoadCompleted(SaveResult result)
        {
            SaveSystemLogger.Log($"Load operation completed. Success: {result.Success}");
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_saveManager != null)
            {
                _saveManager.OnSaveComplete -= OnSaveCompleted;
                _saveManager.OnLoadComplete -= OnLoadCompleted;
            }
        }
    }
}

