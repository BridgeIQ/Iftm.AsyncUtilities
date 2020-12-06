using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Iftm.AsyncUtilities {

    /// <summary>
    /// Used to construct a cancelled <see cref="TaskResult"/> and <see cref="TaskResult{T}"/>.
    /// </summary>
    public struct Cancelled {
    }

    /// <summary>
    /// Represents the result of a task in two properties: <see cref="IsCancelled"/> if the task has been cancelled, and <see cref="Exception"/> if the task
    /// was in <see cref="Task.IsFaulted"/> state.
    /// </summary>
    public readonly struct TaskResult {
        private readonly bool _cancelled;
        private readonly Exception? _exception;

        /// <summary>
        /// Creates a <see cref="TaskResult"/> thats in a cancelled state.
        /// </summary>
        public TaskResult(Cancelled c) {
            _cancelled = true;
            _exception = null;
        }

        /// <summary>
        /// Produces a <see cref="TaskResult"/> with the exception of the task in the <see cref="Task.IsFaulted"/> state.
        /// </summary>
        /// <param name="exception"></param>
        public TaskResult(Exception exception) {
            _cancelled = default;
            _exception = exception;
        }

        /// <summary>
        /// Returns whether the awaited task was canelled.
        /// </summary>
        public bool IsCancelled => _cancelled;

        /// <summary>
        /// Returns whether the awaited task was in the faulted state. Equivalent to <see cref="Exception"/> != null.
        /// </summary>
        public bool IsFaulted => _exception != null;

        /// <summary>
        /// Returns an <see cref="Exception"/> if the task was faulted, or null.
        /// </summary>
        public Exception? Exception => _exception;
    }

    /// <summary>
    /// Represents the result of a task in three properties: <see cref="IsCancelled"/> if the task has been cancelled, <see cref="Exception"/> if the task
    /// was in <see cref="Task.IsFaulted"/> state, and <see cref="Result"/> with the result of the task if not cancelled or faulted.
    /// </summary>
    public readonly struct TaskResult<T> {
        private readonly bool _cancelled;
        private readonly Exception? _exception;
        private readonly T _result;

        /// <summary>
        /// Creates a <see cref="TaskResult{T}"/> with the result of a task that has successfully completed.
        /// </summary>
        /// <param name="result"></param>
        public TaskResult(T result) {
            _cancelled = false;
            _exception = null;
            _result = result;
        }

        /// <summary>
        /// Creates a <see cref="TaskResult{T}"/> thats in a cancelled state.
        /// </summary>
        public TaskResult(Cancelled c) {
            _cancelled = true;
            _exception = null;
            _result = default!;
        }

        /// <summary>
        /// Produces a <see cref="TaskResult{T}"/> with the exception of the task in the <see cref="Task.IsFaulted"/> state.
        /// </summary>
        /// <param name="exception"></param>
        public TaskResult(Exception exception) {
            _cancelled = default;
            _exception = exception;
            _result = default!;
        }

        /// <summary>
        /// Returns whether the awaited task was canelled.
        /// </summary>
        public bool IsCancelled => _cancelled;

        /// <summary>
        /// Returns whether the awaited task was in the faulted state. Equivalent to <see cref="Exception"/> != null.
        /// </summary>
        public bool IsFaulted => _exception != null;

        /// <summary>
        /// Returns an <see cref="Exception"/> if the task was faulted, or null.
        /// </summary>
        public Exception? Exception => _exception;

        /// <summary>
        /// Returns the result of a task if it completed successfully.
        /// </summary>
        public T? Result => _result;
    }

    #pragma warning disable 1591

    /// <summary>
    /// Similar to <see cref="ConfiguredTaskAwaitable"/>, represents an awaitable structure that wraps a task that
    /// once awaited, returns a <see cref="TaskResult"/>.
    /// </summary>
    public struct TaskNoAsyncExceptionAwaitable {
        private readonly Task _task;
        private readonly bool _continueOnCapturedContext;

        /// <summary>
        /// Creates a an awaitable structure from a task that will not throw exceptions when awaited. Instead it will return
        /// a <see cref="TaskResult"/> with properties to signal cancellation, exception or success.
        /// </summary>
        /// <param name="task">Task to be awaited.</param>
        /// <param name="continueOnCapturedContext">Whether the awaiter should continue the action on the same <see cref="System.Threading.SynchronizationContext"/> on which it was awaited.</param>
        public TaskNoAsyncExceptionAwaitable(Task task, bool continueOnCapturedContext) =>
            (_task, _continueOnCapturedContext) = (task, continueOnCapturedContext);

        public Awaiter GetAwaiter() => new Awaiter(_task, _continueOnCapturedContext);

        public struct Awaiter : ICriticalNotifyCompletion {
            private readonly Task _task;
            private readonly bool _continueOnCapturedContext;

            public Awaiter(Task task, bool continueOnCapturedContext) =>
                (_task, _continueOnCapturedContext) = (task, continueOnCapturedContext);
            
            public void UnsafeOnCompleted(Action continuation) => _task.ConfigureAwait(_continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
            public void OnCompleted(Action continuation) => _task.ConfigureAwait(_continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
            public bool IsCompleted => _task.IsCompleted;
            public TaskResult GetResult() =>
                _task.IsCanceled ? new TaskResult(new Cancelled()) :
                _task.IsFaulted ? new TaskResult(_task.Exception.InnerExceptions[0]) :
                new TaskResult();
        }
    }

    public struct TaskNoAsyncExceptionAwaitable<T> {
        private readonly Task<T> _task;
        private readonly bool _continueOnCapturedContext;

        /// <summary>
        /// Creates a an awaitable structure from a task that will not throw exceptions when awaited. Instead it will return
        /// a <see cref="TaskResult{T}"/> with properties to signal cancellation, exception or success.
        /// </summary>
        /// <param name="task">Task to be awaited.</param>
        /// <param name="continueOnCapturedContext">Whether the awaiter should continue the action on the same <see cref="System.Threading.SynchronizationContext"/> on which it was awaited.</param>
        public TaskNoAsyncExceptionAwaitable(Task<T> task, bool continueOnCapturedContext) =>
            (_task, _continueOnCapturedContext) = (task, continueOnCapturedContext);

        /// <summary>
        /// Returns an awaiter.
        /// </summary>
        /// <returns><see cref="Awaiter"/></returns>
        public Awaiter GetAwaiter() => new Awaiter(_task, _continueOnCapturedContext);

        public struct Awaiter : ICriticalNotifyCompletion {
            private readonly Task<T> _task;
            private readonly bool _continueOnCapturedContext;

            public Awaiter(Task<T> task, bool continueOnCapturedContext) =>
                (_task, _continueOnCapturedContext) = (task, continueOnCapturedContext);

            public void UnsafeOnCompleted(Action continuation) => _task.ConfigureAwait(_continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
            public void OnCompleted(Action continuation) => _task.ConfigureAwait(_continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
            public bool IsCompleted => _task.IsCompleted;
            public TaskResult<T> GetResult() =>
                _task.IsCanceled ? new TaskResult<T>(new Cancelled()) :
                _task.IsFaulted ? new TaskResult<T>(_task.Exception.InnerExceptions[0]) :
                new TaskResult<T> (_task.Result);
        }
    }

    public struct ValueTaskNoAsyncExceptionAwaitable {
        private readonly ValueTask _task;
        private readonly bool _continueOnCapturedContext;

        /// <summary>
        /// Creates a an awaitable structure from a task that will not throw exceptions when awaited. Instead it will return
        /// a <see cref="TaskResult"/> with properties to signal cancellation, exception or success.
        /// </summary>
        /// <param name="task">Task to be awaited.</param>
        /// <param name="continueOnCapturedContext">Whether the awaiter should continue the action on the same <see cref="System.Threading.SynchronizationContext"/> on which it was awaited.</param>
        public ValueTaskNoAsyncExceptionAwaitable(ValueTask task, bool continueOnCapturedContext) =>
            (_task, _continueOnCapturedContext) = (task, continueOnCapturedContext);

        /// <summary>
        /// Returns an awaiter.
        /// </summary>
        /// <returns><see cref="Awaiter"/></returns>
        public Awaiter GetAwaiter() => new Awaiter(_task, _continueOnCapturedContext);

        public struct Awaiter : ICriticalNotifyCompletion {
            private readonly ValueTask _task;
            private readonly bool _continueOnCapturedContext;

            public Awaiter(ValueTask task, bool continueOnCapturedContext) =>
                (_task, _continueOnCapturedContext) = (task, continueOnCapturedContext);

            public void UnsafeOnCompleted(Action continuation) => _task.ConfigureAwait(_continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
            public void OnCompleted(Action continuation) => _task.ConfigureAwait(_continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
            public bool IsCompleted => _task.IsCompleted;
            public TaskResult GetResult() =>
                _task.IsCanceled ? new TaskResult(new Cancelled()) :
                _task.IsFaulted ? new TaskResult (_task.AsTask().Exception.InnerExceptions[0]) :
                new TaskResult();
        }
    }

    public struct ValueTaskNoAsyncExceptionAwaitable<T> {
        private readonly ValueTask<T> _task;
        private readonly bool _continueOnCapturedContext;

        /// <summary>
        /// Creates a an awaitable structure from a task that will not throw exceptions when awaited. Instead it will return
        /// a <see cref="TaskResult{T}"/> with properties to signal cancellation, exception or success.
        /// </summary>
        /// <param name="task">Task to be awaited.</param>
        /// <param name="continueOnCapturedContext">Whether the awaiter should continue the action on the same <see cref="System.Threading.SynchronizationContext"/> on which it was awaited.</param>
        public ValueTaskNoAsyncExceptionAwaitable(ValueTask<T> task, bool continueOnCapturedContext) =>
            (_task, _continueOnCapturedContext) = (task, continueOnCapturedContext);

        public Awaiter GetAwaiter() => new Awaiter(_task, _continueOnCapturedContext);

        public struct Awaiter : ICriticalNotifyCompletion {
            private readonly ValueTask<T> _task;
            private readonly bool _continueOnCapturedContext;

            public Awaiter(ValueTask<T> task, bool continueOnCapturedContext) =>
                (_task, _continueOnCapturedContext) = (task, continueOnCapturedContext);

            public void UnsafeOnCompleted(Action continuation) => _task.ConfigureAwait(_continueOnCapturedContext).GetAwaiter().UnsafeOnCompleted(continuation);
            public void OnCompleted(Action continuation) => _task.ConfigureAwait(_continueOnCapturedContext).GetAwaiter().OnCompleted(continuation);
            public bool IsCompleted => _task.IsCompleted;
            public TaskResult<T> GetResult() =>
                _task.IsCanceled ? new TaskResult<T>(new Cancelled()) :
                _task.IsFaulted ? new TaskResult<T>(_task.AsTask().Exception.InnerExceptions[0]) :
                new TaskResult<T>(_task.Result);
        }
    }

    #pragma warning restore 1591

    /// <summary>
    /// Extension class containing the NoAsyncExceptions extension methods.
    /// </summary>
    public static class NoExceptionExtensions {
        /// <summary>
        /// Returns an awaiter that will not throw if an exception happens; Instead it returns a <see cref="TaskResult"/> which has properties 
        /// <see cref="TaskResult.IsCancelled"/> and <see cref="TaskResult.Exception"/> that describe the state of the completed task.
        /// </summary>
        /// <param name="task">Task to be awaited. <see cref="Task"/></param>
        /// <param name="continueOnCapturedContext">Whether execution should continue on the same <see cref="System.Threading.SynchronizationContext"/>. Default is true.</param>
        /// <returns><see cref="TaskNoAsyncExceptionAwaitable"/></returns>
        public static TaskNoAsyncExceptionAwaitable NoAsyncExceptions(this Task task, bool continueOnCapturedContext = true) => new TaskNoAsyncExceptionAwaitable(task, continueOnCapturedContext);

        /// <summary>
        /// Returns an awaiter that will not throw if an exception happens; Instead it returns a <see cref="TaskResult{T}"/> which has properties 
        /// <see cref="TaskResult{T}.IsCancelled"/>, <see cref="TaskResult{T}.Exception"/>, and <see cref="TaskResult{T}.Result"/> that describe the state of the completed task.
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="task">Task to be awaited. <see cref="Task{T}"/></param>
        /// <param name="continueOnCapturedContext">Whether execution should continue on the same <see cref="System.Threading.SynchronizationContext"/>. Default is true.</param>
        /// <returns><see cref="TaskNoAsyncExceptionAwaitable{T}"/></returns>
        public static TaskNoAsyncExceptionAwaitable<T> NoAsyncExceptions<T>(this Task<T> task, bool continueOnCapturedContext = true) => new TaskNoAsyncExceptionAwaitable<T>(task, continueOnCapturedContext);

        /// <summary>
        /// Returns an awaiter that will not throw if an exception happens; Instead it returns a <see cref="TaskResult"/> which has properties 
        /// <see cref="TaskResult.IsCancelled"/> and <see cref="TaskResult.Exception"/> that describe the state of the completed task.
        /// </summary>
        /// <param name="task">Task to be awaited. <see cref="ValueTask"/></param>
        /// <param name="continueOnCapturedContext">Whether execution should continue on the same <see cref="System.Threading.SynchronizationContext"/>. Default is true.</param>
        /// <returns><see cref="ValueTaskNoAsyncExceptionAwaitable"/></returns>
        public static ValueTaskNoAsyncExceptionAwaitable NoAsyncExceptions(this ValueTask task, bool continueOnCapturedContext = true) => new ValueTaskNoAsyncExceptionAwaitable(task, continueOnCapturedContext);

        /// <summary>
        /// Returns an awaiter that will not throw if an exception happens; Instead it returns a <see cref="TaskResult{T}"/> which has properties 
        /// <see cref="TaskResult{T}.IsCancelled"/>, <see cref="TaskResult{T}.Exception"/>, and <see cref="TaskResult{T}.Result"/> that describe the state of the completed task.
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="task">Task to be awaited. <see cref="ValueTask{T}"/></param>
        /// <param name="continueOnCapturedContext">Whether execution should continue on the same <see cref="System.Threading.SynchronizationContext"/>. Default is true.</param>
        /// <returns><see cref="ValueTaskNoAsyncExceptionAwaitable{T}"/></returns>
        public static ValueTaskNoAsyncExceptionAwaitable<T> NoAsyncExceptions<T>(this ValueTask<T> task, bool continueOnCapturedContext = true) => new ValueTaskNoAsyncExceptionAwaitable<T>(task, continueOnCapturedContext);
    }
}
