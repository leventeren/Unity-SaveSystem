using System;

namespace SaveSystem.Models
{
    /// <summary>
    /// Result pattern for save/load operations.
    /// Provides type-safe error handling without exceptions.
    /// </summary>
    /// <typeparam name="T">Type of the result data.</typeparam>
    [Serializable]
    public class SaveResult<T>
    {
        public bool Success { get; private set; }
        public T Data { get; private set; }
        public string ErrorMessage { get; private set; }
        public SaveErrorCode ErrorCode { get; private set; }
        
        private SaveResult() { }
        
        public static SaveResult<T> Ok(T data)
        {
            return new SaveResult<T>
            {
                Success = true,
                Data = data,
                ErrorMessage = null,
                ErrorCode = SaveErrorCode.None
            };
        }
        
        public static SaveResult<T> Fail(string errorMessage, SaveErrorCode errorCode = SaveErrorCode.Unknown)
        {
            return new SaveResult<T>
            {
                Success = false,
                Data = default,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
        }
    }
    
    /// <summary>
    /// Non-generic result for operations without return data.
    /// </summary>
    [Serializable]
    public class SaveResult
    {
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public SaveErrorCode ErrorCode { get; private set; }
        
        private SaveResult() { }
        
        public static SaveResult Ok()
        {
            return new SaveResult
            {
                Success = true,
                ErrorMessage = null,
                ErrorCode = SaveErrorCode.None
            };
        }
        
        public static SaveResult Fail(string errorMessage, SaveErrorCode errorCode = SaveErrorCode.Unknown)
        {
            return new SaveResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
        }
    }
    
    /// <summary>
    /// Error codes for save operations.
    /// </summary>
    public enum SaveErrorCode
    {
        None,
        Unknown,
        FileNotFound,
        SerializationFailed,
        DeserializationFailed,
        ValidationFailed,
        CorruptedData,
        AccessDenied,
        DiskFull,
        NetworkError,
        VersionMismatch
    }
}
