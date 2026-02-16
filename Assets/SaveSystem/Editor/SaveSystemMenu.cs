using UnityEditor;
using UnityEngine;
using System.IO;

namespace SaveSystem.Editor
{
    /// <summary>
    /// Utility menu items for SaveSystem development and debugging.
    /// </summary>
    public static class SaveSystemMenu
    {
        [MenuItem("Tools/SaveSystem/Clear All Saves", priority = 100)]
        public static void ClearAllSaves()
        {
            if (!EditorUtility.DisplayDialog("Clear All Saves",
                "This will delete ALL save files and backups!\n\nThis action cannot be undone!",
                "Delete All", "Cancel"))
                return;
            
            try
            {
                string savesDir = Path.Combine(Application.persistentDataPath, "Saves");
                string backupsDir = Path.Combine(Application.persistentDataPath, "Backups");
                
                if (Directory.Exists(savesDir))
                {
                    Directory.Delete(savesDir, true);
                    Directory.CreateDirectory(savesDir);
                }
                
                if (Directory.Exists(backupsDir))
                {
                    Directory.Delete(backupsDir, true);
                    Directory.CreateDirectory(backupsDir);
                }
                
                Debug.Log("✅ All saves and backups cleared!");
                EditorUtility.DisplayDialog("Success", "All saves and backups have been deleted.", "OK");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to clear saves: {ex.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to clear saves:\n{ex.Message}", "OK");
            }
        }
        
        [MenuItem("Tools/SaveSystem/Open Saves Folder", priority = 101)]
        public static void OpenSavesFolder()
        {
            string savesDir = Path.Combine(Application.persistentDataPath, "Saves");
            
            if (!Directory.Exists(savesDir))
            {
                Directory.CreateDirectory(savesDir);
            }
            
            EditorUtility.RevealInFinder(savesDir);
        }
        
        [MenuItem("Tools/SaveSystem/Open Backups Folder", priority = 102)]
        public static void OpenBackupsFolder()
        {
            string backupsDir = Path.Combine(Application.persistentDataPath, "Backups");
            
            if (!Directory.Exists(backupsDir))
            {
                Directory.CreateDirectory(backupsDir);
            }
            
            EditorUtility.RevealInFinder(backupsDir);
        }
        
        [MenuItem("Tools/SaveSystem/Show Persistent Data Path", priority = 103)]
        public static void ShowPersistentDataPath()
        {
            Debug.Log($"Persistent Data Path: {Application.persistentDataPath}");
            EditorUtility.DisplayDialog("Persistent Data Path", 
                Application.persistentDataPath, "OK");
        }
        
        [MenuItem("Tools/SaveSystem/Generate Test Save", priority = 200)]
        public static void GenerateTestSave()
        {
            string savesDir = Path.Combine(Application.persistentDataPath, "Saves");
            
            if (!Directory.Exists(savesDir))
            {
                Directory.CreateDirectory(savesDir);
            }
            
            string testSavePath = Path.Combine(savesDir, "test_save.json");
            
            string testData = @"{
  ""Version"": 1,
  ""Name"": ""Test Player"",
  ""Level"": 10,
  ""Gold"": 5000,
  ""Experience"": 1234.5,
  ""Health"": 100,
  ""MaxHealth"": 100
}";
            
            try
            {
                File.WriteAllText(testSavePath, testData);
                Debug.Log($"✅ Test save created: {testSavePath}");
                EditorUtility.DisplayDialog("Success", "Test save file created!", "OK");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create test save: {ex.Message}");
            }
        }
        
        [MenuItem("Tools/SaveSystem/Documentation/Open README", priority = 300)]
        public static void OpenReadme()
        {
            // Try multiple possible locations for README.md
            string[] possiblePaths = new string[]
            {
                Path.Combine(Application.dataPath, "..", "README.md"),                    // Project root
                Path.Combine(Application.dataPath, "SaveSystem", "README.md"),             // SaveSystem folder
                Path.Combine(Application.dataPath, "..", "Assets", "SaveSystem", "README.md") // Assets/SaveSystem
            };
            
            string foundPath = null;
            
            foreach (var path in possiblePaths)
            {
                string fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    foundPath = fullPath;
                    break;
                }
            }
            
            if (foundPath != null)
            {
                Application.OpenURL($"file://{foundPath}");
                Debug.Log($"Opening README: {foundPath}");
            }
            else
            {
                Debug.LogWarning("README.md not found! Tried locations:");
                foreach (var path in possiblePaths)
                {
                    Debug.LogWarning($"  - {Path.GetFullPath(path)}");
                }
                
                EditorUtility.DisplayDialog("README Not Found", 
                    "README.md could not be found.\n\nCheck console for attempted paths.", 
                    "OK");
            }
        }
    }
}
