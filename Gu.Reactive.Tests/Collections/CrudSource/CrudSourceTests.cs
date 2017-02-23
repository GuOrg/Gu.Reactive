namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public abstract class CrudSourceTests : IDisposable
    {
#pragma warning disable SA1306 // Field names must begin with lower-case letter
        protected TestExtensions.EventList ActualEventArgs;
        protected TestScheduler Scheduler;
        protected IReadOnlyObservableCollection<int> View;
        protected ObservableCollection<int> Ints;
#pragma warning restore SA1306 // Field names must begin with lower-case letter

        private TestExtensions.EventList expectedEventArgs;
        private bool disposed;

        [SetUp]
        public virtual void SetUp()
        {
            this.Ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
            this.expectedEventArgs?.Dispose();
            this.expectedEventArgs = this.Ints.SubscribeAll();
        }

        [Test]
        public void Ctor()
        {
            CollectionAssert.AreEqual(this.Ints, this.View);
        }

        [Test]
        public void NoChangeNoEvent()
        {
            CollectionAssert.AreEqual(this.Ints, this.View);
            (this.View as IRefreshAble)?.Refresh();
            this.Scheduler?.Start();

            CollectionAssert.AreEqual(this.Ints, this.View);
            CollectionAssert.IsEmpty(this.ActualEventArgs);
        }

        [Test]
        public void UpdatesBeforeItNotifies()
        {
            int[] actual = null;
            int[] expected = null;
            using (this.View.ObserveCollectionChanged(false)
                       .Subscribe(_ => { actual = this.View.ToArray(); }))
            {
                using (this.Ints.ObserveCollectionChanged(false)
                           .Subscribe(_ => { expected = this.Ints.ToArray(); }))
                {
                    this.Ints.Add(5);
                    this.Scheduler?.Start();
                    CollectionAssert.AreEqual(expected, actual);

                    this.Ints.Clear();
                    this.Scheduler?.Start();
                    CollectionAssert.AreEqual(expected, actual);
                }
            }
        }

        [Test]
        public void Add()
        {
            this.Ints.Add(4);
            this.Scheduler?.Start();

            CollectionAssert.AreEqual(this.Ints, this.View);
            CollectionAssert.AreEqual(this.expectedEventArgs, this.ActualEventArgs, EventArgsComparer.Default);
        }

        [Test]
        public void ManyAddsOneReset()
        {
            if (this.Scheduler == null)
            {
                Assert.Inconclusive();
            }

            for (int i = 0; i < 10; i++)
            {
                this.Ints.Add(i);
            }

            this.Scheduler.Start();

            CollectionAssert.AreEqual(this.Ints, this.View);
            CollectionAssert.AreEqual(CachedEventArgs.ResetEventArgsCollection, this.ActualEventArgs, EventArgsComparer.Default);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void Remove(int toRemove)
        {
            this.Ints.Remove(toRemove);
            this.Scheduler?.Start();

            CollectionAssert.AreEqual(this.Ints, this.View);
            CollectionAssert.AreEqual(this.expectedEventArgs, this.ActualEventArgs, EventArgsComparer.Default);
        }

        [TestCase(2, 1)]
        [TestCase(0, 2)]
        public void Replace(int index, int value)
        {
            this.Ints[index] = value;
            this.Scheduler?.Start();

            Assert.AreEqual(value, this.View[index]);
            CollectionAssert.AreEqual(this.Ints, this.View);
            CollectionAssert.AreEqual(this.expectedEventArgs, this.ActualEventArgs, EventArgsComparer.Default);
        }

        [TestCase(0, 1)]
        public void Move(int fromIndex, int toIndex)
        {
            this.Ints.Move(fromIndex, toIndex);
            this.Scheduler?.Start();

            CollectionAssert.AreEqual(this.Ints, this.View);
            CollectionAssert.AreEqual(this.expectedEventArgs, this.ActualEventArgs, EventArgsComparer.Default);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.expectedEventArgs?.Dispose();
                this.ActualEventArgs?.Dispose();
            }
        }

        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}
