using System;

namespace SaveSystem.Models
{
    /// <summary>
    /// Progress information for load/save operations.
    /// </summary>
    [Serializable]
    public class LoadProgressInfo
    {
        /// <summary>
        /// Progress percentage (0-100).
        /// </summary>
        public float Percentage { get; set; }
        
        /// <summary>
        /// Current step description.
        /// </summary>
        public string CurrentStep { get; set; }
        
        /// <summary>
        /// Estimated time remaining in seconds.
        /// </summary>
        public float EstimatedTimeRemaining { get; set; }
        
        public LoadProgressInfo(float percentage, string currentStep, float estimatedTimeRemaining = 0f)
        {
            Percentage = percentage;
            CurrentStep = currentStep;
            EstimatedTimeRemaining = estimatedTimeRemaining;
        }
    }
}
