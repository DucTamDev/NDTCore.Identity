namespace NDTCore.Identity.Contracts.Common
{
    /// <summary>
    /// Represents the result of an operation with success/failure state and optional error messages
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public T? Value { get; private set; }
        public string? Error { get; private set; }
        public List<string> Errors { get; private set; } = new();

        private Result(bool isSuccess, T? value, string? error, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            Errors = errors ?? new List<string>();
        }

        /// <summary>
        /// Creates a successful result with a value
        /// </summary>
        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null);
        }

        /// <summary>
        /// Creates a failed result with a single error message
        /// </summary>
        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, error, new List<string> { error });
        }

        /// <summary>
        /// Creates a failed result with multiple error messages
        /// </summary>
        public static Result<T> Failure(List<string> errors)
        {
            var primaryError = errors.FirstOrDefault() ?? "Operation failed";
            return new Result<T>(false, default, primaryError, errors);
        }

        /// <summary>
        /// Creates a failed result with a single error message
        /// </summary>
        public static Result<T> Failure(string error, List<string> errors)
        {
            return new Result<T>(false, default, error, errors);
        }
    }

    /// <summary>
    /// Non-generic result for operations that don't return a value
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; private set; }
        public List<string> Errors { get; private set; } = new();

        private Result(bool isSuccess, string? error, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            Errors = errors ?? new List<string>();
        }

        /// <summary>
        /// Creates a successful result
        /// </summary>
        public static Result Success()
        {
            return new Result(true, null);
        }

        /// <summary>
        /// Creates a failed result with a single error message
        /// </summary>
        public static Result Failure(string error)
        {
            return new Result(false, error, new List<string> { error });
        }

        /// <summary>
        /// Creates a failed result with multiple error messages
        /// </summary>
        public static Result Failure(List<string> errors)
        {
            var primaryError = errors.FirstOrDefault() ?? "Operation failed";
            return new Result(false, primaryError, errors);
        }

        /// <summary>
        /// Creates a failed result with a single error message
        /// </summary>
        public static Result Failure(string error, List<string> errors)
        {
            return new Result(false, error, errors);
        }
    }
}