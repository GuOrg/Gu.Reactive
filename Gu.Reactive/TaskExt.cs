namespace Gu.Reactive
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class TaskExt
    {
        /// <summary>
        /// http://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="task">
        /// </param>
        /// <param name="cancellationToken">
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                {
                    throw new OperationCanceledException(cancellationToken);
                }
            }

            return await task.ConfigureAwait(false);
        }

        public static Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            return TimeoutAfter(task, (int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// http://blogs.msdn.com/b/pfxteam/archive/2011/11/10/10235834.aspx
        /// </summary>
        /// <param name="task">
        /// </param>
        /// <param name="millisecondsTimeout">
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
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
            var timer = new Timer(
                state =>
            {
                // Recover your state information
                var myTcs = (TaskCompletionSource<VoidTypeStruct>)state;

                // Fault our proxy with a TimeoutException
                myTcs.TrySetException(new TimeoutException());
            }, tcs, millisecondsTimeout, Timeout.Infinite);

            // Wire up the logic for what happens when source task completes
            task.ContinueWith(
                (antecedent, state) =>
            {
                // Recover our state data
                var tuple =
                    (Tuple<Timer, TaskCompletionSource<VoidTypeStruct>>)state;

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

        public static Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
            return TimeoutAfter(task, (int)timeout.TotalMilliseconds);
        }

        /// <summary>
        /// http://blogs.msdn.com/b/pfxteam/archive/2011/11/10/10235834.aspx
        /// </summary>
        /// <param name="task">
        /// </param>
        /// <param name="millisecondsTimeout">
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task<T> TimeoutAfter<T>(this Task<T> task, int millisecondsTimeout)
        {
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
            var timer = new Timer(
                state =>
            {
                // Recover your state information
                var myTcs = (TaskCompletionSource<T>)state;

                // Fault our proxy with a TimeoutException
                myTcs.TrySetException(new TimeoutException());
            }, tcs, millisecondsTimeout, Timeout.Infinite);

            // Wire up the logic for what happens when source task completes
            task.ContinueWith(
                (antecedent, state) =>
            {
                // Recover our state data
                var tuple =
                    (Tuple<Timer, TaskCompletionSource<T>>)state;

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

        /// <summary>
        /// The marshal task results.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="proxy">
        /// The proxy.
        /// </param>
        /// <typeparam name="TResult">
        /// </typeparam>
        internal static void MarshalTaskResults<TResult>(Task source, TaskCompletionSource<TResult> proxy)
        {
            switch (source.Status)
            {
                case TaskStatus.Faulted:
                    proxy.TrySetException(source.Exception);
                    break;
                case TaskStatus.Canceled:
                    proxy.TrySetCanceled();
                    break;
                case TaskStatus.RanToCompletion:
                    Task<TResult> castedSource = source as Task<TResult>;
                    proxy.TrySetResult(castedSource == null
                                            ? default(TResult) // source is a Task
                                            : castedSource.Result); // source is a Task<TResult>
                    break;
            }
        }

        internal struct VoidTypeStruct { }
    }
}
