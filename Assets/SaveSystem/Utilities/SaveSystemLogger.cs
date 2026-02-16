using UnityEngine;
using System.Diagnostics;

namespace SaveSystem.Utilities
{
    /// <summary>
    /// Centralized logging system for SaveSystem.
    /// Automatically disabled in production builds.
    /// </summary>
    public static class SaveSystemLogger
    {
        private static bool _verboseLogging = false;
        
        /// <summary>
        /// Enable or disable verbose logging (debug/info messages).
        /// Warnings and errors are always logged.
        /// </summary>
        public static bool VerboseLogging
        {
            get => _verboseLogging;
            set => _verboseLogging = value;
        }
        
        /// <summary>
        /// Log informational message (disabled in production).
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string message)
        {
            if (_verboseLogging)
            {
                UnityEngine.Debug.Log($"[SaveSystem] {message}");
            }
        }
        
        /// <summary>
        /// Log informational message with context (disabled in production).
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(string message, Object context)
        {
            if (_verboseLogging)
            {
                UnityEngine.Debug.Log($"[SaveSystem] {message}", context);
            }
        }
        
        /// <summary>
        /// Log warning message (always enabled).
        /// </summary>
        public static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning($"[SaveSystem] {message}");
        }
        
        /// <summary>
        /// Log warning message with context (always enabled).
        /// </summary>
        public static void LogWarning(string message, Object context)
        {
            UnityEngine.Debug.LogWarning($"[SaveSystem] {message}", context);
        }
        
        /// <summary>
        /// Log error message (always enabled).
        /// </summary>
        public static void LogError(string message)
        {
            UnityEngine.Debug.LogError($"[SaveSystem] {message}");
        }
        
        /// <summary>
        /// Log error message with context (always enabled).
        /// </summary>
        public static void LogError(string message, Object context)
        {
            UnityEngine.Debug.LogError($"[SaveSystem] {message}", context);
        }
        
        /// <summary>
        /// Log debug message (only in editor, always disabled in builds).
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public static void LogDebug(string message)
        {
            if (_verboseLogging)
            {
                UnityEngine.Debug.Log($"[SaveSystem] [DEBUG] {message}");
            }
        }
        
        /// <summary>
        /// Log successful operation (disabled in production).
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogSuccess(string message)
        {
            if (_verboseLogging)
            {
                UnityEngine.Debug.Log($"[SaveSystem] ✅ {message}");
            }
        }
        
        /// <summary>
        /// Log platform information (disabled in production).
        /// </summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogPlatformInfo(string platform, string storage)
        {
            if (_verboseLogging)
            {
                UnityEngine.Debug.Log($"[SaveSystem] Platform: {platform}, Storage: {storage}");
            }
        }
    }
}
