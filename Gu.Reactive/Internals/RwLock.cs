namespace Gu.Reactive.Internals
{
    using System;
    using System.Threading;

    public sealed class RwLock : IDisposable
    {
        private readonly ReaderWriterLockSlim _innerLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private bool _disposed = false;

        //public RwLock(LockRecursionPolicy recursionPolicy)
        //{
        //    _innerLock = new ReaderWriterLockSlim(recursionPolicy);
        //}

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
            if (!_disposed)
            {
                _disposed = true;
                _innerLock.Dispose();
            }
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
                _rwLock.ExitReadLock();
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
                _rwLock.ExitUpgradeableReadLock();
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
                _rwLock.ExitWriteLock();
            }
        }
    }
}
