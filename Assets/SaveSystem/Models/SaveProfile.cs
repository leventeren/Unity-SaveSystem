using System;

namespace SaveSystem.Models
{
    /// <summary>
    /// Represents a save profile/slot.
    /// </summary>
    [Serializable]
    public class SaveProfile
    {
        /// <summary>
        /// Unique profile identifier.
        /// </summary>
        public string ProfileName { get; set; }
        
        /// <summary>
        /// Display name for UI.
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Profile metadata.
        /// </summary>
        public SaveMetadata Metadata { get; set; }
        
        /// <summary>
        /// Is this profile currently active.
        /// </summary>
        public bool IsActive { get; set; }
        
        public SaveProfile(string profileName)
        {
            ProfileName = profileName;
            DisplayName = profileName;
            Metadata = new SaveMetadata();
            IsActive = false;
        }
    }
}
