# Unity SaveSystem

🎮 **Production-ready, SOLID-compliant Save System for Unity**

A comprehensive, modular, and extensible save/load system for Unity games with advanced features like auto-save, backup management, versioning, and more.

## ✨ Features

### Core Features
- ✅ **Generic Type Support** - `SaveAsync<T>` and `LoadAsync<T>`
- ✅ **Multiple Serialization Formats** - JSON (Unity JsonUtility), Binary
- ✅ **Multiple Storage Strategies** - Local File System, PlayerPrefs
- ✅ **Async/Await Operations** - Non-blocking I/O
- ✅ **Progress Tracking** - Real-time save/load progress callbacks
- ✅ **Event System** - OnSaveComplete, OnLoadComplete events
- ✅ **Result Pattern** - Type-safe error handling without exceptions
- ✅ **Cross-Platform** - Works on PC, Mobile (iOS/Android)

### Advanced Features
- ⚡ **Auto-Save System** - Configurable interval-based automatic saving
- 📦 **Backup & Recovery** - Automatic backup rotation (keeps last N backups)
- 🔄 **Versioning & Migration** - Backward compatibility for save format changes
- 🗜️ **Compression** - GZip compression for 60-80% file size reduction
- 🔒 **Corruption Detection** - Checksum validation (CRC32, MD5, SHA256)
- ✅ **Data Validation** - Business rule validation before/after serialization

### Design
- 🏗️ **SOLID Principles** - Clean, maintainable architecture
- 🎯 **Strategy Pattern** - Easily swap serialization and storage implementations
- 🔨 **Builder Pattern** - Fluent API for configuration
- 🎭 **Façade Pattern** - Simple API hiding complex operations
- 📝 **Result Pattern** - Safe error handling
- 💉 **DI Container Support** - Zenject/VContainer ready
- ⚙️ **ScriptableObject Configuration** - Inspector-driven setup (Recommended!)
- 🔄 **Automatic Update Loop** - No manual Update() calls needed

## 🚀 Quick Start

### Recommended: ScriptableObject Configuration

**The easiest way to use SaveSystem:**

1. **Create Settings Asset:**
   - Right-click in Project → Create → SaveSystem → Settings
   - Name it `SaveSystemSettings`

2. **Configure in Inspector:**
   ```
   Default Profile Name: Player
   Serializer Type: JSON
   Storage Type: AutoDetect
   
   ✓ Enable Auto Save (interval: 300s)
   ✓ Enable Backup (max: 3)
   ✓ Use Compression
   ✓ Verbose Logging
   ```

3. **One-Line Setup:**
   ```csharp
   using SaveSystem;
   using UnityEngine;

   public class GameManager : MonoBehaviour
   {
       [SerializeField] private SaveSystemSettings settings;
       private SaveManager _saveManager;
       
       void Start()
       {
           // That's it! Everything configured automatically!
           _saveManager = new SaveManagerBuilder()
               .UseSettings(settings)
               .Build();
       }
       
       // Save - backup created automatically!
       public async void SaveGame()
       {
           var result = await _saveManager.SaveAsync("Player", playerData);
       }
   }
   ```

**AutoSave works automatically - no Update() needed!**

### Alternative: Manual Configuration

```csharp
using SaveSystem;
using SaveSystem.Examples;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private SaveManager _saveManager;
    
    void Start()
    {
        // Setup SaveManager with fluent builder
        _saveManager = new SaveManagerBuilder()
            .UseJsonSerializer()         // or .UseBinarySerializer()
            .UseLocalFileStorage()       // or .UsePlayerPrefsStorage()
            .Build();
    }
    
    // Save game
    async void SaveGame()
    {
        var playerData = new PlayerData 
        { 
            Name = "Hero", 
            Level = 10, 
            Gold = 5000 
        };
        
        var result = await _saveManager.SaveAsync("slot1", playerData);
        
        if (result.Success)
            Debug.Log("Game saved!");
        else
            Debug.LogError($"Save failed: {result.ErrorMessage}");
    }
    
    // Load game
    async void LoadGame()
    {
        var result = await _saveManager.LoadAsync<PlayerData>("slot1");
        
        if (result.Success)
        {
            var data = result.Data;
            Debug.Log($"Loaded: {data.Name}, Level {data.Level}");
        }
    }
}
```

### With Progress Callbacks

```csharp
async void SaveWithProgress()
{
    var progress = new Progress<float>(p => 
    {
        Debug.Log($"Saving: {p * 100:F0}%");
        // Update UI progress bar
    });
    
    var result = await _saveManager.SaveAsync("slot1", playerData, progress);
}
```

### Complex Data Types (List & Dictionary)

```csharp
using SaveSystem.Core;
using SaveSystem.Models;
using System.Collections.Generic;

[Serializable]
public class PlayerInventory : ISaveData
{
    public int Version => 1;
    
    public List<Card> OwnedCards;
    public SerializableDictionary<string, int> CardCounts;
    
    public PlayerInventory()
    {
        OwnedCards = new List<Card>();
        CardCounts = new SerializableDictionary<string, int>();
    }
    
    public bool Validate()
    {
        return OwnedCards != null && CardCounts != null;
    }
    
    public string GetChecksum()
    {
        // Implement checksum calculation
        return "";
    }
}
```

### UI Button Integration

**Unity buttons require `async void` methods:**

```csharp
public class GameController : MonoBehaviour
{
    private SaveManager _saveManager;
    
    // Can be called from Unity UI Button!
    public async void OnSaveButtonClick()
    {
        var result = await _saveManager.SaveAsync("slot1", data);
        
        if (result.Success)
            statusText.text = "Saved!";
    }
}
```

**Button Setup:**
1. Add Button component
2. Inspector: Button → OnClick() → Select script
3. Choose `async void` method

### Backup & Recovery

```csharp
using SaveSystem.Features.Backup;

// Create backup manager (keeps last 3 backups)
var backupManager = new BackupManager(_saveManager, maxBackupCount: 3);

// Create backup
await backupManager.CreateBackupAsync("profile1");

// List backups
var backups = backupManager.GetBackups("profile1");

// Restore from most recent backup
await backupManager.RestoreFromBackupAsync("profile1", backupIndex: 0);
```

### Versioning & Migration

```csharp
using SaveSystem.Features.Versioning;

// Setup migration manager
var migrationManager = new VersionMigrationManager();

// Register migration from v1 to v2
migrationManager.RegisterMigration<PlayerData>(
    fromVersion: 1,
    toVersion: 2,
    migrationAction: data =>
    {
        // Add default value for new field in v2
        data.NewFieldInV2 = "default_value";
    }
);

// Migration happens automatically when loading old saves
var result = await _saveManager.LoadAsync<PlayerData>("old_save");
```

### Zenject Integration

**Using SaveSystem with Zenject dependency injection:**

#### 1. Setup Installer

```csharp
using SaveSystem.Installers;
using Zenject;

// Add SaveSystemInstaller to your SceneContext
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Install SaveSystem
        SaveSystemInstaller.Install(Container);
    }
}
```

Or use the MonoInstaller directly:
```
Hierarchy:
└── SceneContext
    └── SaveSystemInstaller (Component)
```

#### 2. Inject Dependencies

```csharp
using SaveSystem;
using SaveSystem.Features.AutoSave;
using SaveSystem.Features.Backup;
using Zenject;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private SaveManager _saveManager;
    private AutoSaveManager _autoSave;
    private BackupManager _backup;
    
    // Constructor injection
    [Inject]
    public void Construct(
        SaveManager saveManager,
        AutoSaveManager autoSave,
        BackupManager backup)
    {
        _saveManager = saveManager;
        _autoSave = autoSave;
        _backup = backup;
    }
    
    async void SaveGame()
    {
        await _saveManager.SaveAsync("profile", playerData);
    }
}
```

#### 3. Platform-Specific Bindings

```csharp
public override void InstallBindings()
{
    // Different strategies per platform
    #if UNITY_ANDROID || UNITY_IOS
        Container.Bind<ISaveStrategy>()
            .To<PlayerPrefsSaveStrategy>()
            .AsSingle();
    #else
        Container.Bind<ISaveStrategy>()
            .To<LocalFileSaveStrategy>()
            .AsSingle();
    #endif
}
```

#### 4. Custom Configuration

```csharp
// Configure in Inspector (SaveSystemInstaller component)
- Use Json Serializer: ✓
- Use Compression: ✓
- Enable Auto Save: ✓
- Auto Save Interval: 300
- Max Backup Count: 3
```

**Benefits:**
- ✅ Testability - Easy to mock dependencies
- ✅ Flexibility - Runtime configuration
- ✅ Decoupling - Clean architecture
- ✅ Scene Independence - Global bindings

## 📚 Examples

The package includes several example scripts:

- **`SettingsBasedExample.cs`** ⭐ **RECOMMENDED** - ScriptableObject configuration (easiest!)
- **`CrossPlatformExample.cs`** - Platform auto-detection, UI integration
- **`SaveSystemExample.cs`** - Basic save/load operations
- **`AdvancedFeaturesExample.cs`** - Auto-save and backup demos
- **`ZenjectSaveExample.cs`** - Zenject dependency injection usage
- **`PlayerData.cs`** - Simple data example
- **`PlayerInventory.cs`** - Complex data with List and Dictionary

## 📁 Architecture

```
SaveSystem/
├── Configuration/         # SaveSystemSettings, SaveSystemEnums ⭐
├── Core/Interfaces/       # Core interfaces (ISaveData, ISerializer, ISaveStrategy)
├── Models/                # Data models (SaveResult, SaveMetadata)
├── Serialization/         # Serializers (JSON, Binary)
├── Storage/               # Storage strategies (LocalFile, PlayerPrefs)
├── Compression/           # Compression (GZip)
├── Features/
│   ├── AutoSave/          # Auto-save system with timer
│   ├── Backup/            # Backup & recovery with rotation
│   ├── Versioning/        # Migration system
│   └── Validation/        # Corruption detection
├── Utilities/             # SaveSystemLogger, UpdateProxy
├── Editor/                # Editor tools (File Viewer, Slot Management)
├── Installers/            # Zenject DI installers
├── SaveManager.cs         # Main façade - auto backup/autosave
├── SaveManagerBuilder.cs  # Fluent builder - .WithBackup(), .WithAutoSave()
└── Examples/              # 7 usage examples
```

**Total: 40 files, 6000+ lines of code**

## 🔧 Configuration

### Different Serializers

```csharp
// JSON (human-readable, debug-friendly)
.UseJsonSerializer()

// Binary (compact, faster)
.UseBinarySerializer()

// Custom serializer
.UseSerializer(new MyCustomSerializer())
```

### Different Storage

```csharp
// Local file system (recommended for most cases)
.UseLocalFileStorage()

// PlayerPrefs (simple, cross-platform, limited size)
.UsePlayerPrefsStorage()

// Custom storage
.UseStorage(new MyCustomStorage())
```

## 🎓 Creating Custom Save Data

Implement `ISaveData` interface:

```csharp
using SaveSystem.Core;
using System;

[Serializable]
public class MyGameData : ISaveData
{
    public int Version => 1;  // Increment when format changes
    
    public string PlayerName;
    public int Level;
    
    // Validate business rules
    public bool Validate()
    {
        return !string.IsNullOrEmpty(PlayerName) && Level >= 1;
    }
    
    // Calculate checksum for corruption detection
    public string GetChecksum()
    {
        // Use CorruptionDetector helper or implement your own
        return "";
    }
}
```

## ⚡ Performance

- **Async I/O** - Non-blocking operations, no frame drops
- **Compression** - 60-80% file size reduction with GZip
- **Atomic Writes** - Temp file strategy prevents corruption
- **Progress Callbacks** - Smooth loading screens

## 🔒 Security

- **Checksum Validation** - CRC32, MD5, SHA256 support
- **Data Integrity** - Pre/post serialization validation
- **Corruption Detection** - Automatic checksum verification
- **Backup System** - Protection against data loss

## 📝 License

Free to use in personal and commercial projects.

## 🤝 Contributing

This is a self-contained save system. Feel free to extend and customize for your needs!

## 📖 Documentation

See `walkthrough.md` for detailed implementation guide and architecture overview.

---

**Latest Features:**
- ✅ ScriptableObject Configuration System
- ✅ Automatic Backup on Save
- ✅ Automatic AutoSave Update Loop  
- ✅ UI Button Integration Pattern
- ✅ Production-Safe Logging System

**Made with ❤️ for Unity developers | 40 Files | 6000+ LOC**
