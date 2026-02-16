using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

namespace SaveSystem.Editor
{
    /// <summary>
    /// Editor window for viewing and managing save files.
    /// Provides file browsing, JSON viewing, and validation testing.
    /// </summary>
    public class SaveFileViewerWindow : EditorWindow
    {
        private string _savesDirectory;
        private string[] _saveFiles;
        private int _selectedFileIndex = -1;
        private string _fileContent = "";
        private Vector2 _scrollPosition;
        private Vector2 _fileListScroll;
        
        [MenuItem("Tools/SaveSystem/Save File Viewer")]
        public static void ShowWindow()
        {
            var window = GetWindow<SaveFileViewerWindow>("Save File Viewer");
            window.minSize = new Vector2(800, 600);
        }
        
        private void OnEnable()
        {
            _savesDirectory = Path.Combine(Application.persistentDataPath, "Saves");
            RefreshFileList();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            
            // Header
            DrawHeader();
            
            EditorGUILayout.Space(10);
            
            // Main content area
            EditorGUILayout.BeginHorizontal();
            
            // Left panel: File list
            DrawFileListPanel();
            
            EditorGUILayout.Space(5);
            
            // Right panel: File content
            DrawFileContentPanel();
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Save File Viewer", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Directory: {_savesDirectory}", EditorStyles.miniLabel);
            
            if (GUILayout.Button("Open Folder", GUILayout.Width(100)))
            {
                EditorUtility.RevealInFinder(_savesDirectory);
            }
            
            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
            {
                RefreshFileList();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        
        private void DrawFileListPanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            
            EditorGUILayout.LabelField("Save Files", EditorStyles.boldLabel);
            
            if (_saveFiles == null || _saveFiles.Length == 0)
            {
                EditorGUILayout.HelpBox("No save files found.", MessageType.Info);
            }
            else
            {
                _fileListScroll = EditorGUILayout.BeginScrollView(_fileListScroll);
                
                for (int i = 0; i < _saveFiles.Length; i++)
                {
                    string fileName = Path.GetFileName(_saveFiles[i]);
                    FileInfo fileInfo = new FileInfo(_saveFiles[i]);
                    
                    EditorGUILayout.BeginVertical("box");
                    
                    bool isSelected = i == _selectedFileIndex;
                    var style = new GUIStyle(GUI.skin.button)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        fontStyle = isSelected ? FontStyle.Bold : FontStyle.Normal
                    };
                    
                    if (GUILayout.Button(fileName, style))
                    {
                        SelectFile(i);
                    }
                    
                    EditorGUILayout.LabelField($"Size: {FormatFileSize(fileInfo.Length)}", EditorStyles.miniLabel);
                    EditorGUILayout.LabelField($"Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm}", EditorStyles.miniLabel);
                    
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(2);
                }
                
                EditorGUILayout.EndScrollView();
            }
            
            EditorGUILayout.Space(5);
            
            if (GUILayout.Button("Delete All Saves", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Delete All Saves",
                    "Are you sure you want to delete all save files? This cannot be undone!",
                    "Delete All", "Cancel"))
                {
                    DeleteAllSaves();
                }
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void DrawFileContentPanel()
        {
            EditorGUILayout.BeginVertical();
            
            if (_selectedFileIndex >= 0 && _selectedFileIndex < _saveFiles.Length)
            {
                string fileName = Path.GetFileName(_saveFiles[_selectedFileIndex]);
                EditorGUILayout.LabelField($"Viewing: {fileName}", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Copy to Clipboard", GUILayout.Width(130)))
                {
                    EditorGUIUtility.systemCopyBuffer = _fileContent;
                    Debug.Log("Save file content copied to clipboard!");
                }
                
                if (GUILayout.Button("Delete File", GUILayout.Width(100)))
                {
                    if (EditorUtility.DisplayDialog("Delete Save File",
                        $"Delete {fileName}?", "Delete", "Cancel"))
                    {
                        DeleteFile(_selectedFileIndex);
                    }
                }
                
                if (GUILayout.Button("Validate JSON", GUILayout.Width(110)))
                {
                    ValidateJSON();
                }
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(5);
                
                // Content viewer
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                
                GUIStyle textStyle = new GUIStyle(EditorStyles.textArea)
                {
                    wordWrap = true
                };
                
                _fileContent = EditorGUILayout.TextArea(_fileContent, textStyle, 
                    GUILayout.ExpandHeight(true));
                
                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Select a save file to view its contents.", MessageType.Info);
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void RefreshFileList()
        {
            if (!Directory.Exists(_savesDirectory))
            {
                Directory.CreateDirectory(_savesDirectory);
            }
            
            _saveFiles = Directory.GetFiles(_savesDirectory, "*.json")
                .Concat(Directory.GetFiles(_savesDirectory, "*.sav"))
                .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                .ToArray();
            
            _selectedFileIndex = -1;
            _fileContent = "";
        }
        
        private void SelectFile(int index)
        {
            _selectedFileIndex = index;
            
            try
            {
                _fileContent = File.ReadAllText(_saveFiles[index]);
            }
            catch (System.Exception ex)
            {
                _fileContent = $"Error reading file: {ex.Message}";
                EditorUtility.DisplayDialog("Error", $"Failed to read file: {ex.Message}", "OK");
            }
        }
        
        private void DeleteFile(int index)
        {
            try
            {
                File.Delete(_saveFiles[index]);
                RefreshFileList();
                Debug.Log($"Deleted save file: {Path.GetFileName(_saveFiles[index])}");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to delete file: {ex.Message}", "OK");
            }
        }
        
        private void DeleteAllSaves()
        {
            try
            {
                foreach (var file in _saveFiles)
                {
                    File.Delete(file);
                }
                
                RefreshFileList();
                Debug.Log("All save files deleted.");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to delete files: {ex.Message}", "OK");
            }
        }
        
        private void ValidateJSON()
        {
            try
            {
                JsonUtility.FromJson<object>(_fileContent);
                EditorUtility.DisplayDialog("Validation", "JSON is valid!", "OK");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Validation Failed", 
                    $"Invalid JSON:\n{ex.Message}", "OK");
            }
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
}
