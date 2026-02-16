using UnityEditor;
using UnityEngine;
using SaveSystem.Features.Backup;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SaveSystem.Editor
{
    /// <summary>
    /// Editor window for managing save slots.
    /// Provides slot preview, copy, delete, and rename operations.
    /// </summary>
    public class SlotManagementWindow : EditorWindow
    {
        private string _savesDirectory;
        private List<SlotInfo> _slots = new List<SlotInfo>();
        private Vector2 _scrollPosition;
        private BackupManager _backupManager;
        
        private class SlotInfo
        {
            public string FilePath;
            public string ProfileName;
            public long FileSize;
            public System.DateTime LastModified;
            public int BackupCount;
        }
        
        [MenuItem("Tools/SaveSystem/Slot Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<SlotManagementWindow>("Slot Manager");
            window.minSize = new Vector2(600, 400);
        }
        
        private void OnEnable()
        {
            _savesDirectory = Path.Combine(Application.persistentDataPath, "Saves");
            RefreshSlots();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            
            // Header
            DrawHeader();
            
            EditorGUILayout.Space(10);
            
            // Slots list
            DrawSlotsList();
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Save Slot Manager", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Directory: {_savesDirectory}", EditorStyles.miniLabel);
            
            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
            {
                RefreshSlots();
            }
            
            if (GUILayout.Button("Open Folder", GUILayout.Width(100)))
            {
                EditorUtility.RevealInFinder(_savesDirectory);
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        
        private void DrawSlotsList()
        {
            if (_slots.Count == 0)
            {
                EditorGUILayout.HelpBox("No save slots found.", MessageType.Info);
                return;
            }
            
            EditorGUILayout.LabelField($"Found {_slots.Count} save slot(s)", EditorStyles.boldLabel);
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            foreach (var slot in _slots)
            {
                DrawSlotCard(slot);
                EditorGUILayout.Space(5);
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void DrawSlotCard(SlotInfo slot)
        {
            EditorGUILayout.BeginVertical("box");
            
            // Title
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(slot.ProfileName, EditorStyles.boldLabel);
            
            // Action buttons
            if (GUILayout.Button("Copy", GUILayout.Width(60)))
            {
                CopySlot(slot);
            }
            
            if (GUILayout.Button("Rename", GUILayout.Width(70)))
            {
                RenameSlot(slot);
            }
            
            if (GUILayout.Button("Delete", GUILayout.Width(70)))
            {
                DeleteSlot(slot);
            }
            
            if (GUILayout.Button("View", GUILayout.Width(60)))
            {
                ViewSlot(slot);
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Metadata
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"📁 Size: {FormatFileSize(slot.FileSize)}", 
                EditorStyles.miniLabel, GUILayout.Width(120));
            EditorGUILayout.LabelField($"🕒 Modified: {slot.LastModified:yyyy-MM-dd HH:mm}", 
                EditorStyles.miniLabel, GUILayout.Width(200));
            
            if (slot.BackupCount > 0)
            {
                EditorGUILayout.LabelField($"💾 Backups: {slot.BackupCount}", 
                    EditorStyles.miniLabel, GUILayout.Width(100));
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Backup management
            if (slot.BackupCount > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                
                if (GUILayout.Button("View Backups", GUILayout.Width(110)))
                {
                    ViewBackups(slot);
                }
                
                if (GUILayout.Button("Restore Latest Backup", GUILayout.Width(160)))
                {
                    RestoreBackup(slot);
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void RefreshSlots()
        {
            _slots.Clear();
            
            if (!Directory.Exists(_savesDirectory))
            {
                Directory.CreateDirectory(_savesDirectory);
                return;
            }
            
            var files = Directory.GetFiles(_savesDirectory, "*.json")
                .Concat(Directory.GetFiles(_savesDirectory, "*.sav"))
                .ToArray();
            
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                string profileName = Path.GetFileNameWithoutExtension(file);
                
                // Count backups
                int backupCount = CountBackups(profileName);
                
                _slots.Add(new SlotInfo
                {
                    FilePath = file,
                    ProfileName = profileName,
                    FileSize = fileInfo.Length,
                    LastModified = fileInfo.LastWriteTime,
                    BackupCount = backupCount
                });
            }
            
            _slots = _slots.OrderByDescending(s => s.LastModified).ToList();
        }
        
        private int CountBackups(string profileName)
        {
            string backupDir = Path.Combine(Application.persistentDataPath, "Backups", profileName);
            
            if (!Directory.Exists(backupDir))
                return 0;
            
            return Directory.GetFiles(backupDir, "*.bak").Length;
        }
        
        private void CopySlot(SlotInfo slot)
        {
            string newName = $"{slot.ProfileName}_copy";
            string newPath = Path.Combine(_savesDirectory, newName + Path.GetExtension(slot.FilePath));
            
            int counter = 1;
            while (File.Exists(newPath))
            {
                newName = $"{slot.ProfileName}_copy{counter}";
                newPath = Path.Combine(_savesDirectory, newName + Path.GetExtension(slot.FilePath));
                counter++;
            }
            
            try
            {
                File.Copy(slot.FilePath, newPath);
                RefreshSlots();
                Debug.Log($"Slot copied: {newName}");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to copy slot: {ex.Message}", "OK");
            }
        }
        
        private void RenameSlot(SlotInfo slot)
        {
            string newName = EditorInputDialog.Show("Rename Slot", "Enter new name:", slot.ProfileName);
            
            if (string.IsNullOrEmpty(newName) || newName == slot.ProfileName)
                return;
            
            string newPath = Path.Combine(_savesDirectory, newName + Path.GetExtension(slot.FilePath));
            
            try
            {
                File.Move(slot.FilePath, newPath);
                RefreshSlots();
                Debug.Log($"Slot renamed: {slot.ProfileName} → {newName}");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to rename slot: {ex.Message}", "OK");
            }
        }
        
        private void DeleteSlot(SlotInfo slot)
        {
            if (!EditorUtility.DisplayDialog("Delete Slot", 
                $"Delete save slot '{slot.ProfileName}'?\n\nThis will also delete all backups!", 
                "Delete", "Cancel"))
                return;
            
            try
            {
                // Delete save file
                File.Delete(slot.FilePath);
                
                // Delete backups
                string backupDir = Path.Combine(Application.persistentDataPath, "Backups", slot.ProfileName);
                if (Directory.Exists(backupDir))
                {
                    Directory.Delete(backupDir, true);
                }
                
                RefreshSlots();
                Debug.Log($"Slot deleted: {slot.ProfileName}");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to delete slot: {ex.Message}", "OK");
            }
        }
        
        private void ViewSlot(SlotInfo slot)
        {
            SaveFileViewerWindow.ShowWindow();
        }
        
        private void ViewBackups(SlotInfo slot)
        {
            string backupDir = Path.Combine(Application.persistentDataPath, "Backups", slot.ProfileName);
            EditorUtility.RevealInFinder(backupDir);
        }
        
        private void RestoreBackup(SlotInfo slot)
        {
            if (!EditorUtility.DisplayDialog("Restore Backup",
                $"Restore latest backup for '{slot.ProfileName}'?\n\nCurrent save will be overwritten!",
                "Restore", "Cancel"))
                return;
            
            // This is a simplified version - in practice would use BackupManager
            Debug.LogWarning("Backup restore requires runtime BackupManager. Use the backup system at runtime.");
            EditorUtility.DisplayDialog("Info", 
                "Backup restore is available at runtime via BackupManager.\n\n" +
                "Use: await backupManager.RestoreFromBackupAsync(profileName, 0);", 
                "OK");
        }
        
        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            
            return $"{len:0.##} {sizes[order]}";
        }
    }
    
    /// <summary>
    /// Simple input dialog for editor.
    /// </summary>
    public class EditorInputDialog : EditorWindow
    {
        private string _inputText = "";
        private string _description = "";
        private System.Action<string> _onConfirm;
        
        public static string Show(string title, string description, string defaultValue = "")
        {
            var window = GetWindow<EditorInputDialog>(true, title, true);
            window._description = description;
            window._inputText = defaultValue;
            window.minSize = new Vector2(300, 100);
            window.maxSize = new Vector2(300, 100);
            window.ShowModalUtility();
            
            return window._inputText;
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField(_description);
            _inputText = EditorGUILayout.TextField(_inputText);
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("OK"))
            {
                Close();
            }
            
            if (GUILayout.Button("Cancel"))
            {
                _inputText = "";
                Close();
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}
