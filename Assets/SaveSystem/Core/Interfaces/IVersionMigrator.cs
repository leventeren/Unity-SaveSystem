namespace SaveSystem.Core
{
    /// <summary>
    /// Interface for save data version migration.
    /// Handles backward compatibility when save format changes.
    /// </summary>
    public interface IVersionMigrator
    {
        /// <summary>
        /// Checks if this migrator can handle the migration from a specific version.
        /// </summary>
        /// <param name="fromVersion">Source version.</param>
        /// <param name="toVersion">Target version.</param>
        /// <returns>True if migration is supported.</returns>
        bool CanMigrate(int fromVersion, int toVersion);
        
        /// <summary>
        /// Migrates data from one version to another.
        /// </summary>
        /// <typeparam name="T">Type of save data.</typeparam>
        /// <param name="data">Data to migrate.</param>
        /// <param name="fromVersion">Source version.</param>
        /// <param name="toVersion">Target version.</param>
        /// <returns>Migrated data.</returns>
        T Migrate<T>(T data, int fromVersion, int toVersion) where T : ISaveData;
    }
}
