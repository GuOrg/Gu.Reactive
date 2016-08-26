namespace Gu.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Threading;

    /// <summary>
    /// Sets thread to foreground while there are items in the queue.
    /// Useful for save operations that should keep the app alive until finished.
    /// </summary>
    public class ForegroundScheduler : IScheduler
    {
        public static readonly ForegroundScheduler Default = new ForegroundScheduler();
        private readonly EventLoopScheduler inner;
        private Thread thread;
        private int count;

        private ForegroundScheduler()
        {
            this.inner = new EventLoopScheduler(this.CreateThread);
        }

        public DateTimeOffset Now => this.inner.Now;

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            Interlocked.Increment(ref this.count);
            return this.inner.Schedule(state, (sc, st) => this.Invoke(sc, st, action));
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            Interlocked.Increment(ref this.count);
            return this.inner.Schedule(state, dueTime, (sc, st) => this.Invoke(sc, st, action));
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            Interlocked.Increment(ref this.count);
            return this.inner.Schedule(state, dueTime, (sc, st) => this.Invoke(sc, st, action));
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
                Interlocked.Decrement(ref this.count);
                this.thread.IsBackground = this.count == 0;
            }

            return disposable;
        }

        private Thread CreateThread(ThreadStart arg)
        {
            this.thread = new Thread(arg)
                          {
                              Name = "ForegroundScheduler",
                              IsBackground = true // maybe we want it as foreground when saving?
                          };
            return this.thread;
        }
    }
}