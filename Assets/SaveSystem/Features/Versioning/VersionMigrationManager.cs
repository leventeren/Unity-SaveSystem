using SaveSystem.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem.Features.Versioning
{
    /// <summary>
    /// Manages save data version migrations.
    /// Ensures backward compatibility when save format changes.
    /// </summary>
    public class VersionMigrationManager : IVersionMigrator
    {
        private readonly Dictionary<Type, List<IMigrationStep>> _migrations;
        
        public VersionMigrationManager()
        {
            _migrations = new Dictionary<Type, List<IMigrationStep>>();
        }
        
        /// <summary>
        /// Registers a migration step for a specific type.
        /// </summary>
        public void RegisterMigration<T>(int fromVersion, int toVersion, Action<T> migrationAction) where T : ISaveData
        {
            Type type = typeof(T);
            
            if (!_migrations.ContainsKey(type))
            {
                _migrations[type] = new List<IMigrationStep>();
            }
            
            _migrations[type].Add(new MigrationStep<T>(fromVersion, toVersion, migrationAction));
            
            Debug.Log($"Registered migration for {type.Name}: v{fromVersion} → v{toVersion}");
        }
        
        public bool CanMigrate(int fromVersion, int toVersion)
        {
            // Can migrate if fromVersion < toVersion
            return fromVersion < toVersion;
        }
        
        public T Migrate<T>(T data, int fromVersion, int toVersion) where T : ISaveData
        {
            if (fromVersion == toVersion)
            {
                Debug.Log($"No migration needed: already at version {toVersion}");
                return data;
            }
            
            if (!CanMigrate(fromVersion, toVersion))
            {
                throw new InvalidOperationException($"Cannot migrate from v{fromVersion} to v{toVersion}");
            }
            
            Type type = typeof(T);
            
            if (!_migrations.ContainsKey(type))
            {
                Debug.LogWarning($"No migrations registered for {type.Name}");
                return data;
            }
            
            // Sort migrations by fromVersion
            var migrations = _migrations[type];
            migrations.Sort((a, b) => a.FromVersion.CompareTo(b.FromVersion));
            
            // Apply migrations in sequence
            int currentVersion = fromVersion;
            
            foreach (var migration in migrations)
            {
                if (migration.FromVersion == currentVersion && migration.ToVersion <= toVersion)
                {
                    Debug.Log($"Applying migration: v{migration.FromVersion} → v{migration.ToVersion}");
                    migration.Apply(data);
                    currentVersion = migration.ToVersion;
                }
            }
            
            if (currentVersion != toVersion)
            {
                Debug.LogWarning($"Migration incomplete: reached v{currentVersion}, expected v{toVersion}");
            }
            
            return data;
        }
        
        // Internal migration step interface
        private interface IMigrationStep
        {
            int FromVersion { get; }
            int ToVersion { get; }
            void Apply(object data);
        }
        
        // Internal migration step implementation
        private class MigrationStep<T> : IMigrationStep where T : ISaveData
        {
            public int FromVersion { get; }
            public int ToVersion { get; }
            private readonly Action<T> _migrationAction;
            
            public MigrationStep(int fromVersion, int toVersion, Action<T> migrationAction)
            {
                FromVersion = fromVersion;
                ToVersion = toVersion;
                _migrationAction = migrationAction;
            }
            
            public void Apply(object data)
            {
                if (data is T typedData)
                {
                    _migrationAction(typedData);
                }
            }
        }
    }
}
