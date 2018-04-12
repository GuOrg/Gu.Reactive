namespace Gu.Reactive.Internals
{
    using System;
    using System.Threading;

    /// <summary>
    /// A wrapper of <see cref="ReaderWriterLockSlim"/>
    /// </summary>
    public sealed class RwLock : IDisposable
    {
        private readonly ReaderWriterLockSlim innerLock;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RwLock"/> class.
        /// Creates a RwLock with LockRecursionPolicy.NoRecursion
        /// </summary>
        public RwLock()
            : this(LockRecursionPolicy.NoRecursion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RwLock"/> class.
        /// </summary>
        public RwLock(LockRecursionPolicy recursionPolicy)
        {
            this.innerLock = new ReaderWriterLockSlim(recursionPolicy);
        }

        /// <summary>
        /// Acquire the read lock.
        /// </summary>
        public IDisposable Read()
        {
            this.ThrowIfDisposed();
            return new Reader(this.innerLock);
        }

        /// <summary>
        /// Acquire the upgradable read lock.
        /// </summary>
        public IDisposable UpgradeableRead()
        {
            this.ThrowIfDisposed();
            return new UpgradeableReader(this.innerLock);
        }

        /// <summary>
        /// Acquire the write lock.
        /// </summary>
        public IDisposable Write()
        {
            this.ThrowIfDisposed();
            return new Writer(this.innerLock);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.innerLock.Dispose();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"RwLock RecursionPolicy: {this.innerLock.RecursionPolicy}, IsReadLockHeld:{this.innerLock.IsReadLockHeld}, IsWriteLockHeld: {this.innerLock.IsWriteLockHeld}, IsUpgradeableReadLockHeld: {this.innerLock.IsUpgradeableReadLockHeld}";
        }

        private void ThrowIfDisposed()
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
                rwLock.EnterReadLock();
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
                return $"RwLock.Writer RecursionPolicy: {this.rwLock.RecursionPolicy}, IsReadLockHeld:{this.rwLock.IsReadLockHeld}, IsWriteLockHeld: {this.rwLock.IsWriteLockHeld}, IsUpgradeableReadLockHeld: {this.rwLock.IsUpgradeableReadLockHeld}";
            }
        }

        private class UpgradeableReader : IDisposable
        {
            private readonly ReaderWriterLockSlim rwLock;

            public UpgradeableReader(ReaderWriterLockSlim rwLock)
            {
                this.rwLock = rwLock;
                rwLock.EnterUpgradeableReadLock();
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
                return $"RwLock.Writer RecursionPolicy: {this.rwLock.RecursionPolicy}, IsReadLockHeld:{this.rwLock.IsReadLockHeld}, IsWriteLockHeld: {this.rwLock.IsWriteLockHeld}, IsUpgradeableReadLockHeld: {this.rwLock.IsUpgradeableReadLockHeld}";
            }
        }

        private class Writer : IDisposable
        {
            private readonly ReaderWriterLockSlim rwLock;

            public Writer(ReaderWriterLockSlim rwLock)
            {
                this.rwLock = rwLock;
                rwLock.EnterWriteLock();
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
                return $"RwLock.Writer RecursionPolicy: {this.rwLock.RecursionPolicy}, IsReadLockHeld:{this.rwLock.IsReadLockHeld}, IsWriteLockHeld: {this.rwLock.IsWriteLockHeld}, IsUpgradeableReadLockHeld: {this.rwLock.IsUpgradeableReadLockHeld}";
            }
        }
    }
}
