namespace Gu.Reactive.Internals
{
    using System;
    using System.Threading;

    public sealed class RwLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _innerLock;
        private bool _disposed;

        /// <summary>
        /// Creates a RwLock with LockRecursionPolicy.NoRecursion
        /// </summary>
        public RwLock()
            : this(LockRecursionPolicy.NoRecursion)
        {
        }

        public RwLock(LockRecursionPolicy recursionPolicy)
        {
            _innerLock = new ReaderWriterLockSlim(recursionPolicy);
        }

        public IDisposable Read()
        {
            VerifyDisposed();
            return new Reader(_innerLock);
        }

        public IDisposable UpgradeableRead()
        {
            VerifyDisposed();
            return new UpgradeableReader(_innerLock);
        }

        public IDisposable Write()
        {
            VerifyDisposed();
            return new Writer(_innerLock);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _innerLock.Dispose();
        }

        public override string ToString()
        {
            return string.Format(
                "RwLock RecursionPolicy: {0}, IsReadLockHeld:{1}, IsWriteLockHeld: {2}, IsUpgradeableReadLockHeld: {3}",
                _innerLock.RecursionPolicy,
                _innerLock.IsReadLockHeld,
                _innerLock.IsWriteLockHeld,
                _innerLock.IsUpgradeableReadLockHeld);
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private class Reader : IDisposable
        {
            private readonly ReaderWriterLockSlim _rwLock;
            public Reader(ReaderWriterLockSlim rwLock)
            {
                _rwLock = rwLock;
                _rwLock.EnterReadLock();
            }

            public void Dispose()
            {
                if (!_rwLock.IsReadLockHeld)
                {
                    return;
                }
                _rwLock.ExitReadLock();
            }

            public override string ToString()
            {
                return string.Format(
                    "RwLock.Writer RecursionPolicy: {0}, IsReadLockHeld:{1}, IsWriteLockHeld: {2}, IsUpgradeableReadLockHeld: {3}",
                    _rwLock.RecursionPolicy,
                    _rwLock.IsReadLockHeld,
                    _rwLock.IsWriteLockHeld,
                    _rwLock.IsUpgradeableReadLockHeld);
            }
        }

        private class UpgradeableReader : IDisposable
        {
            private readonly ReaderWriterLockSlim _rwLock;
            public UpgradeableReader(ReaderWriterLockSlim rwLock)
            {
                _rwLock = rwLock;
                _rwLock.EnterUpgradeableReadLock();
            }

            public void Dispose()
            {
                if (!_rwLock.IsUpgradeableReadLockHeld)
                {
                    return;
                }
                _rwLock.ExitUpgradeableReadLock();
            }

            public override string ToString()
            {
                return string.Format(
                    "RwLock.Writer RecursionPolicy: {0}, IsReadLockHeld:{1}, IsWriteLockHeld: {2}, IsUpgradeableReadLockHeld: {3}",
                    _rwLock.RecursionPolicy,
                    _rwLock.IsReadLockHeld,
                    _rwLock.IsWriteLockHeld,
                    _rwLock.IsUpgradeableReadLockHeld);
            }
        }

        private class Writer : IDisposable
        {
            private readonly ReaderWriterLockSlim _rwLock;
            public Writer(ReaderWriterLockSlim rwLock)
            {
                _rwLock = rwLock;
                _rwLock.EnterWriteLock();
            }

            public void Dispose()
            {
                if (!_rwLock.IsWriteLockHeld)
                {
                    return;
                }
                _rwLock.ExitWriteLock();
            }

            public override string ToString()
            {
                return string.Format(
                    "RwLock.Writer RecursionPolicy: {0}, IsReadLockHeld:{1}, IsWriteLockHeld: {2}, IsUpgradeableReadLockHeld: {3}",
                    _rwLock.RecursionPolicy,
                    _rwLock.IsReadLockHeld,
                    _rwLock.IsWriteLockHeld,
                    _rwLock.IsUpgradeableReadLockHeld);
            }
        }
    }
}
