namespace Gu.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Threading;

    /// <summary>
    /// Sets thread to foreground while there are items in the queue.
    /// Useful for save operations that should keep the app alive until finished.
    /// </summary>
    public sealed class ForegroundScheduler : IScheduler, IDisposable
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        public static readonly ForegroundScheduler Default = new ForegroundScheduler();
        private readonly EventLoopScheduler inner;
        private Thread thread;
        private int count;
        private bool disposed;

        private ForegroundScheduler()
        {
            this.inner = new EventLoopScheduler(this.CreateThread);
        }

        /// <inheritdoc/>
        public DateTimeOffset Now => this.inner.Now;

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            this.ThrowIfDisposed();
#pragma warning disable GU0011 // Don't ignore the return value.
            Interlocked.Increment(ref this.count);
#pragma warning restore GU0011 // Don't ignore the return value.
            return this.inner.Schedule(state, (sc, st) => this.Invoke(sc, st, action));
        }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            this.ThrowIfDisposed();
#pragma warning disable GU0011 // Don't ignore the return value.
            Interlocked.Increment(ref this.count);
#pragma warning restore GU0011 // Don't ignore the return value.
            return this.inner.Schedule(state, dueTime, (sc, st) => this.Invoke(sc, st, action));
        }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            this.ThrowIfDisposed();
#pragma warning disable GU0011 // Don't ignore the return value.
            Interlocked.Increment(ref this.count);
#pragma warning restore GU0011 // Don't ignore the return value.
            return this.inner.Schedule(state, dueTime, (sc, st) => this.Invoke(sc, st, action));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.inner.Dispose();
        }

        private IDisposable Invoke<TState>(IScheduler scheduler, TState state, Func<IScheduler, TState, IDisposable> action)
        {
            this.thread.IsBackground = false;
            IDisposable disposable;
            try
            {
                disposable = action(scheduler, state);
            }
            finally
            {
                var n = Interlocked.Decrement(ref this.count);
                this.thread.IsBackground = n == 0;
            }

            return disposable;
        }

        private Thread CreateThread(ThreadStart arg)
        {
            this.thread = new Thread(arg)
            {
                Name = "ForegroundScheduler",
                IsBackground = true, // maybe we want it as foreground when saving?
            };
            return this.thread;
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(typeof(ForegroundScheduler).FullName);
            }
        }
    }
}
