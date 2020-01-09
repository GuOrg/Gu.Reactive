#pragma warning disable UseAsyncSuffix // Use Async suffix
namespace Gu.Reactive
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Helpers for <see cref="Task"/>.
    /// </summary>
    public static class TaskExt
    {
        /// <summary>
        /// Make a task ~cancelable~
        /// The inner task will still complete after canceling so side-effects may be an issue.
        /// http://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx.
        /// </summary>
        /// <typeparam name="T">The type of the task.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var tcs = new TaskCompletionSource<bool>();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            {
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            }

            return await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Decorate a task with timeout.
        /// The inner task will still complete after canceling so side-effects may be an issue.
        /// http://blogs.msdn.com/b/pfxteam/archive/2011/11/10/10235834.aspx.
        /// </summary>
        public static Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return TimeoutAfter(task, (int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// Decorate a task with timeout.
        /// The inner task will still complete after canceling so side-effects may be an issue.
        /// http://blogs.msdn.com/b/pfxteam/archive/2011/11/10/10235834.aspx.
        /// </summary>
        public static Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            // Short-circuit #1: infinite timeout or task already completed
            if (task.IsCompleted || (millisecondsTimeout == Timeout.Infinite))
            {
                // Either the task has already completed or timeout will never occur.
                // No proxy necessary.
                return task;
            }

            // tcs.Task will be returned as a proxy to the caller
            var tcs = new TaskCompletionSource<VoidTypeStruct>();

            // Short-circuit #2: zero timeout
            if (millisecondsTimeout == 0)
            {
                // We've already timed out.
                tcs.SetException(new TimeoutException());
                return tcs.Task;
            }

            // Set up a timer to complete after the specified timeout period
#pragma warning disable IDISP001  // Dispose created.
            var timer = new Timer(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                callback: state => ((TaskCompletionSource<VoidTypeStruct>)state).TrySetException(new TimeoutException()),
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                state: tcs,
                dueTime: millisecondsTimeout,
                period: Timeout.Infinite);
#pragma warning restore IDISP001  // Dispose created.

            // Wire up the logic for what happens when source task completes
            _ = task.ContinueWith(
                (antecedent, state) =>
                    {
                        // Recover our state data
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                        var tuple = (Tuple<Timer, TaskCompletionSource<VoidTypeStruct>>)state;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                        // Cancel the Timer
                        tuple!.Item1.Dispose();

                        // Marshal results to proxy
                        MarshalTaskResults(antecedent, tuple.Item2);
                    },
                Tuple.Create(timer, tcs),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            return tcs.Task;
        }

        /// <summary>
        /// Decorate a task with timeout.
        /// The inner task will still complete after canceling so side-effects may be an issue.
        /// http://blogs.msdn.com/b/pfxteam/archive/2011/11/10/10235834.aspx.
        /// </summary>
        public static Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            return TimeoutAfter(task, (int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// Decorate a task with timeout.
        /// The inner task will still complete after canceling so side-effects may be an issue.
        /// http://blogs.msdn.com/b/pfxteam/archive/2011/11/10/10235834.aspx.
        /// </summary>
        public static Task<T> TimeoutAfter<T>(this Task<T> task, int millisecondsTimeout)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            // Short-circuit #1: infinite timeout or task already completed
            if (task.IsCompleted || (millisecondsTimeout == Timeout.Infinite))
            {
                // Either the task has already completed or timeout will never occur.
                // No proxy necessary.
                return task;
            }

            // tcs.Task will be returned as a proxy to the caller
            var tcs = new TaskCompletionSource<T>();

            // Short-circuit #2: zero timeout
            if (millisecondsTimeout == 0)
            {
                // We've already timed out.
                tcs.SetException(new TimeoutException());
                return tcs.Task;
            }

            // Set up a timer to complete after the specified timeout period
#pragma warning disable IDISP001  // Dispose created.
            var timer = new Timer(
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                callback: state => ((TaskCompletionSource<T>)state).TrySetException(new TimeoutException()),
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                state: tcs,
                dueTime: millisecondsTimeout,
                period: Timeout.Infinite);
#pragma warning restore IDISP001  // Dispose created.

            // Wire up the logic for what happens when source task completes
            _ = task.ContinueWith(
                (antecedent, state) =>
                    {
                        // Recover our state data
                        var tuple = (Tuple<Timer, TaskCompletionSource<T>>)state!;

                        // Cancel the Timer
                        tuple.Item1.Dispose();

                        // Marshal results to proxy
                        MarshalTaskResults(antecedent, tuple.Item2);
                    },
                Tuple.Create(timer, tcs),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            return tcs.Task;
        }

        internal static void MarshalTaskResults<TResult>(Task source, TaskCompletionSource<TResult> proxy)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (proxy is null)
            {
                throw new ArgumentNullException(nameof(proxy));
            }

            switch (source.Status)
            {
                case TaskStatus.Faulted:
                    // ReSharper disable once AssignNullToNotNullAttribute
                    proxy.TrySetException(source.Exception!);
                    break;
                case TaskStatus.Canceled:
                    proxy.TrySetCanceled();
                    break;
                case TaskStatus.RanToCompletion:
                    if (source is Task<TResult> generic)
                    {
                        _ = proxy.TrySetResult(generic.Result);
                    }
                    else
                    {
                        _ = proxy.TrySetResult(default!);
                    }

                    break;
            }
        }

        internal struct VoidTypeStruct
        {
        }
    }
}
