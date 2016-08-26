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
        private readonly EventLoopScheduler _inner;
        private Thread _thread;
        private int _count;

        private ForegroundScheduler()
        {
            _inner = new EventLoopScheduler(CreateThread);
        }

        public DateTimeOffset Now => _inner.Now;

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            Interlocked.Increment(ref _count);
            return _inner.Schedule(state, (sc, st) => Invoke(sc, st, action));
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            Interlocked.Increment(ref _count);
            return _inner.Schedule(state, dueTime, (sc, st) => Invoke(sc, st, action));
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            Interlocked.Increment(ref _count);
            return _inner.Schedule(state, dueTime, (sc, st) => Invoke(sc, st, action));
        }

        private IDisposable Invoke<TState>(IScheduler scheduler, TState state, Func<IScheduler, TState, IDisposable> action)
        {
            _thread.IsBackground = false;
            IDisposable disposable;
            try
            {
                disposable = action(scheduler, state);
            }
            finally
            {
                Interlocked.Decrement(ref _count);
                _thread.IsBackground = _count == 0;
            }

            return disposable;
        }

        private Thread CreateThread(ThreadStart arg)
        {
            _thread = new Thread(arg)
                          {
                              Name = "ForegroundScheduler",
                              IsBackground = true // maybe we want it as foreground when saving?
                          };
            return _thread;
        }
    }
}