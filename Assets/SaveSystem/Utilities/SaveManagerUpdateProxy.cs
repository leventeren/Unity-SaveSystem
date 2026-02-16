using UnityEngine;

namespace SaveSystem.Utilities
{
    /// <summary>
    /// Internal singleton that automatically updates AutoSaveManager.
    /// Created automatically by SaveManagerBuilder when AutoSave is enabled.
    /// </summary>
    internal class SaveManagerUpdateProxy : MonoBehaviour
    {
        private static SaveManagerUpdateProxy _instance;
        private SaveManager _saveManager;
        
        /// <summary>
        /// Get or create the singleton instance.
        /// </summary>
        internal static SaveManagerUpdateProxy GetOrCreate()
        {
            if (_instance == null)
            {
                var go = new GameObject("[SaveSystem UpdateProxy]");
                _instance = go.AddComponent<SaveManagerUpdateProxy>();
                DontDestroyOnLoad(go);
                
                #if UNITY_EDITOR
                go.hideFlags = HideFlags.HideInHierarchy; // Hide in hierarchy
                #endif
            }
            
            return _instance;
        }
        
        /// <summary>
        /// Register a SaveManager for automatic updates.
        /// </summary>
        internal void RegisterSaveManager(SaveManager saveManager)
        {
            _saveManager = saveManager;
        }
        
        private void Update()
        {
            // Update AutoSaveManager if present
            if (_saveManager?.AutoSaveManager != null)
            {
                _saveManager.AutoSaveManager.Update(Time.deltaTime);
            }
        }
        
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
