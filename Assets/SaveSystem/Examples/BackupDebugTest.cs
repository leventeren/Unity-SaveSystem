using SaveSystem;
using SaveSystem.Examples;
using UnityEngine;

namespace SaveSystem.Examples
{
    /// <summary>
    /// Quick debug test for backup functionality.
    /// </summary>
    public class BackupDebugTest : MonoBehaviour
    {
        private void Start()
        {
            TestBackupPaths();
        }
        
        [ContextMenu("Test Backup Paths")]
        private void TestBackupPaths()
        {
            string persistentPath = Application.persistentDataPath;
            string savesPath = System.IO.Path.Combine(persistentPath, "Saves", "Player.json");
            string backupsPath = System.IO.Path.Combine(persistentPath, "Backups");
            
            Debug.Log("=== Backup Debug Info ===");
            Debug.Log($"Persistent Path: {persistentPath}");
            Debug.Log($"Expected Save File: {savesPath}");
            Debug.Log($"Save File Exists: {System.IO.File.Exists(savesPath)}");
            Debug.Log($"Backups Path: {backupsPath}");
            Debug.Log($"Backups Dir Exists: {System.IO.Directory.Exists(backupsPath)}");
            
            if (System.IO.Directory.Exists(backupsPath))
            {
                var files = System.IO.Directory.GetFiles(backupsPath, "*.*", System.IO.SearchOption.AllDirectories);
                Debug.Log($"Backup Files Count: {files.Length}");
                foreach (var file in files)
                {
                    Debug.Log($"  - {file}");
                }
            }
            
            Debug.Log("========================");
        }
    }
}
