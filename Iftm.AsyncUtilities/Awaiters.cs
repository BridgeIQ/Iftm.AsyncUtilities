using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Iftm.AsyncUtilities {

    public struct TaskResult {
        private readonly bool _cancelled;
        private readonly Exception? _exception;

        public TaskResult(bool cancelled) {
            _cancelled = cancelled;
            _exception = null;
        }

        public TaskResult(Exception exception) {
            _cancelled = default;
            _exception = exception;
        }

        public bool IsCancelled => _cancelled;
        public bool IsFaulted => _exception != null;
        public Exception? Exception => _exception;
    }

    public struct TaskResult<T> {
        private readonly bool _cancelled;
        private readonly Exception? _exception;
        private readonly T _calue;

        public TaskResult(T value) {
            _cancelled = false;
            _exception = null;
            _calue = value;
        }

        public TaskResult(bool cancelled) {
            _cancelled = cancelled;
            _exception = null;
            _calue = default!;
        }

        public TaskResult(Exception exception) {
            _cancelled = default;
            _exception = exception;
            _calue = default!;
        }

        public bool IsCancelled => _cancelled;
        public bool IsFaulted => _exception != null;
        public Exception? Exception => _exception;
        public T Value => _calue;
    }

    public struct TaskNoAsyncExceptionAwaitable {
        private readonly Task _task;
        private readonly bool _continueOnCapturedContext;

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
                _task.IsCanceled ? new TaskResult(true) :
                _task.IsFaulted ? new TaskResult(_task.Exception.InnerExceptions[0]) :
                new TaskResult();
        }
    }

    public struct TaskNoAsyncExceptionAwaitable<T> {
        private readonly Task<T> _task;
        private readonly bool _continueOnCapturedContext;

        public TaskNoAsyncExceptionAwaitable(Task<T> task, bool continueOnCapturedContext) =>
            (_task, _continueOnCapturedContext) = (task, continueOnCapturedContext);

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
                _task.IsCanceled ? new TaskResult<T>(true) :
                _task.IsFaulted ? new TaskResult<T>(_task.Exception.InnerExceptions[0]) :
                new TaskResult<T> (_task.Result);
        }
    }

    public struct ValueTaskNoAsyncExceptionAwaitable {
        private readonly ValueTask _task;
        private readonly bool _continueOnCapturedContext;

        public ValueTaskNoAsyncExceptionAwaitable(ValueTask task, bool continueOnCapturedContext) =>
            (_task, _continueOnCapturedContext) = (task, continueOnCapturedContext);

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
                _task.IsCanceled ? new TaskResult(true) :
                _task.IsFaulted ? new TaskResult (_task.AsTask().Exception.InnerExceptions[0]) :
                new TaskResult();
        }
    }

    public struct ValueTaskNoAsyncExceptionAwaitable<T> {
        private readonly ValueTask<T> _task;
        private readonly bool _continueOnCapturedContext;

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
                _task.IsCanceled ? new TaskResult<T>(true) :
                _task.IsFaulted ? new TaskResult<T>(_task.AsTask().Exception.InnerExceptions[0]) :
                new TaskResult<T>(_task.Result);
        }
    }

    public static class NoExceptionExtensions {
        public static TaskNoAsyncExceptionAwaitable NoAsyncExceptions(this Task task, bool continueOnCapturedContext = true) => new TaskNoAsyncExceptionAwaitable(task, continueOnCapturedContext);
        public static TaskNoAsyncExceptionAwaitable<T> NoAsyncExceptions<T>(this Task<T> task, bool continueOnCapturedContext = true) => new TaskNoAsyncExceptionAwaitable<T>(task, continueOnCapturedContext);
        public static ValueTaskNoAsyncExceptionAwaitable NoAsyncExceptions(this ValueTask task, bool continueOnCapturedContext = true) => new ValueTaskNoAsyncExceptionAwaitable(task, continueOnCapturedContext);
        public static ValueTaskNoAsyncExceptionAwaitable<T> NoAsyncExceptions<T>(this ValueTask<T> task, bool continueOnCapturedContext = true) => new ValueTaskNoAsyncExceptionAwaitable<T>(task, continueOnCapturedContext);
    }
}
