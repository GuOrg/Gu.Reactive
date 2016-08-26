namespace Gu.Reactive.Internals
{
    using System;
    using System.Threading;

    public sealed class RwLock : IDisposable
    {
        private readonly ReaderWriterLockSlim innerLock;
        private bool disposed;

        /// <summary>
        /// Creates a RwLock with LockRecursionPolicy.NoRecursion
        /// </summary>
        public RwLock()
            : this(LockRecursionPolicy.NoRecursion)
        {
        }

        public RwLock(LockRecursionPolicy recursionPolicy)
        {
            this.innerLock = new ReaderWriterLockSlim(recursionPolicy);
        }

        public IDisposable Read()
        {
            this.VerifyDisposed();
            return new Reader(this.innerLock);
        }

        public IDisposable UpgradeableRead()
        {
            this.VerifyDisposed();
            return new UpgradeableReader(this.innerLock);
        }

        public IDisposable Write()
        {
            this.VerifyDisposed();
            return new Writer(this.innerLock);
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.innerLock.Dispose();
        }

        public override string ToString()
        {
            return string.Format(
                "RwLock RecursionPolicy: {0}, IsReadLockHeld:{1}, IsWriteLockHeld: {2}, IsUpgradeableReadLockHeld: {3}",
                this.innerLock.RecursionPolicy,
                this.innerLock.IsReadLockHeld,
                this.innerLock.IsWriteLockHeld,
                this.innerLock.IsUpgradeableReadLockHeld);
        }

        private void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private class Reader : IDisposable
        {
            private readonly ReaderWriterLockSlim rwLock;

            public Reader(ReaderWriterLockSlim rwLock)
            {
                this.rwLock = rwLock;
                this.rwLock.EnterReadLock();
            }

            public void Dispose()
            {
                if (!this.rwLock.IsReadLockHeld)
                {
                    return;
                }

                this.rwLock.ExitReadLock();
            }

            public override string ToString()
            {
                return string.Format(
                    "RwLock.Writer RecursionPolicy: {0}, IsReadLockHeld:{1}, IsWriteLockHeld: {2}, IsUpgradeableReadLockHeld: {3}",
                    this.rwLock.RecursionPolicy,
                    this.rwLock.IsReadLockHeld,
                    this.rwLock.IsWriteLockHeld,
                    this.rwLock.IsUpgradeableReadLockHeld);
            }
        }

        private class UpgradeableReader : IDisposable
        {
            private readonly ReaderWriterLockSlim rwLock;

            public UpgradeableReader(ReaderWriterLockSlim rwLock)
            {
                this.rwLock = rwLock;
                this.rwLock.EnterUpgradeableReadLock();
            }

            public void Dispose()
            {
                if (!this.rwLock.IsUpgradeableReadLockHeld)
                {
                    return;
                }

                this.rwLock.ExitUpgradeableReadLock();
            }

            public override string ToString()
            {
                return string.Format(
                    "RwLock.Writer RecursionPolicy: {0}, IsReadLockHeld:{1}, IsWriteLockHeld: {2}, IsUpgradeableReadLockHeld: {3}",
                    this.rwLock.RecursionPolicy,
                    this.rwLock.IsReadLockHeld,
                    this.rwLock.IsWriteLockHeld,
                    this.rwLock.IsUpgradeableReadLockHeld);
            }
        }

        private class Writer : IDisposable
        {
            private readonly ReaderWriterLockSlim rwLock;

            public Writer(ReaderWriterLockSlim rwLock)
            {
                this.rwLock = rwLock;
                this.rwLock.EnterWriteLock();
            }

            public void Dispose()
            {
                if (!this.rwLock.IsWriteLockHeld)
                {
                    return;
                }

                this.rwLock.ExitWriteLock();
            }

            public override string ToString()
            {
                return string.Format(
                    "RwLock.Writer RecursionPolicy: {0}, IsReadLockHeld:{1}, IsWriteLockHeld: {2}, IsUpgradeableReadLockHeld: {3}",
                    this.rwLock.RecursionPolicy,
                    this.rwLock.IsReadLockHeld,
                    this.rwLock.IsWriteLockHeld,
                    this.rwLock.IsUpgradeableReadLockHeld);
            }
        }
    }
}
