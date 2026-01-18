using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Application.Common
{
    /// <summary>
    /// Represents the result of an operation that can succeed or fail, and optionally contains a value of the specified
    /// type when successful.
    /// </summary>
    /// <remarks>Use this type to encapsulate the outcome of an operation along with an optional result value
    /// and error information. When the operation is successful, the Value property contains the result; otherwise, the
    /// Error property provides details about the failure.</remarks>
    /// <typeparam name="T">The type of the value returned by a successful operation.</typeparam>

    public class ResultWrapper<T> : ResultWrapper
    {
        public T? Value { get; }

        protected ResultWrapper(bool isSuccess, T? value, Error? error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static ResultWrapper<T> Success(T value)
            => new(true, value, null);

        public static new ResultWrapper<T> Failure(Error error)
            => new(false, default, error);
    }

    /// <summary>
    /// Represents the outcome of an operation, indicating success or failure and providing error details if applicable. 
    /// Use when the opperation has no return value.
    /// </summary>
    /// <remarks>Use the static methods <see cref="Success"/> and <see cref="Failure"/> to create instances
    /// representing successful or failed results. This type is commonly used to encapsulate the result of operations
    /// that may fail, allowing callers to check the <see cref="IsSuccess"/> property and access error information
    /// through the <see cref="Error"/> property if needed.</remarks>

    public class ResultWrapper
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error? Error { get; }

        protected ResultWrapper(bool isSuccess, Error? error)
        {
            if (isSuccess && error != null)
                throw new InvalidOperationException();

            if (!isSuccess && error == null)
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = error;
        }

        public static ResultWrapper Success()
            => new(true, null);

        public static ResultWrapper Failure(Error error)
            => new(false, error);
    }

}
