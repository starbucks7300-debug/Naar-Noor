using System;

namespace NaarNoor.Desktop.Common.Utilities
{
    /// <summary>
    /// Represents the result of an operation without a value.
    /// Used for tracking success/failure and error messages.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Whether the operation succeeded
        /// </summary>
        public bool IsSuccess { get; protected set; }

        /// <summary>
        /// Error message if operation failed
        /// </summary>
        public string? Error { get; protected set; }

        protected Result(bool isSuccess, string? error = null)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// Create a successful result
        /// </summary>
        public static Result Success() => new Result(true);

        /// <summary>
        /// Create a failed result with an error message
        /// </summary>
        public static Result Failure(string error) => new Result(false, error);
    }

    /// <summary>
    /// Represents the result of an operation that returns a value.
    /// Used for tracking success/failure, error messages, and the returned value.
    /// </summary>
    /// <typeparam name="T">Type of the value returned on success</typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// The value returned by a successful operation
        /// </summary>
        public T? Value { get; private set; }

        protected Result(bool isSuccess, T? value = default, string? error = null) : base(isSuccess, error)
        {
            Value = value;
        }

        /// <summary>
        /// Create a successful result with a value
        /// </summary>
        public static Result<T> Success(T value) => new Result<T>(true, value);

        /// <summary>
        /// Create a failed result with an error message
        /// </summary>
        public static new Result<T> Failure(string error) => new Result<T>(false, error: error);

        /// <summary>
        /// Transform the value if successful
        /// </summary>
        public Result<U> Map<U>(Func<T, U> selector)
        {
            if (!IsSuccess || Value == null)
            {
                return Result<U>.Failure(Error ?? "Operation failed");
            }
            return Result<U>.Success(selector(Value));
        }

        /// <summary>
        /// Chain operations that return Result<U>
        /// </summary>
        public Result<U> Bind<U>(Func<T, Result<U>> binder)
        {
            if (!IsSuccess || Value == null)
            {
                return Result<U>.Failure(Error ?? "Operation failed");
            }
            return binder(Value);
        }
    }
}
