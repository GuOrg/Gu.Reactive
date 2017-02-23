namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public abstract class CrudSourceTests
    {
#pragma warning disable SA1306 // Field names must begin with lower-case letter
        protected TestScheduler Scheduler;
        protected IReadOnlyObservableCollection<int> View;
        protected ObservableCollection<int> Ints;
#pragma warning restore SA1306 // Field names must begin with lower-case letter

        [SetUp]
        public virtual void SetUp()
        {
            this.Ints = new ObservableCollection<int>(new[] { 1, 2, 3 });
        }

        [Test]
        public void Ctor()
        {
            CollectionAssert.AreEqual(this.Ints, this.View);
        }

        [Test]
        public void NoChangeNoEvent()
        {
            using (var actual = this.View.SubscribeAll())
            {
                CollectionAssert.AreEqual(this.Ints, this.View);
                (this.View as IRefreshAble)?.Refresh();
                this.Scheduler?.Start();

                CollectionAssert.AreEqual(this.Ints, this.View);
                CollectionAssert.IsEmpty(actual);
            }
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
            using (var expected = this.Ints.SubscribeAll())
            {
                using (var actual = this.View.SubscribeAll())
                {
                    this.Ints.Add(4);
                    this.Scheduler?.Start();

                    CollectionAssert.AreEqual(this.Ints, this.View);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void ManyAddsOneReset()
        {
            if (this.Scheduler == null)
            {
                Assert.Inconclusive();
            }

            using (var actual = this.View.SubscribeAll())
            {
                for (int i = 0; i < 10; i++)
                {
                    this.Ints.Add(i);
                }

                this.Scheduler.Start();

                CollectionAssert.AreEqual(this.Ints, this.View);
                CollectionAssert.AreEqual(CachedEventArgs.ResetEventArgsCollection, actual, EventArgsComparer.Default);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        public void Remove(int toRemove)
        {
            using (var expected = this.Ints.SubscribeAll())
            {
                using (var actual = this.View.SubscribeAll())
                {
                    this.Ints.Remove(toRemove);
                    this.Scheduler?.Start();

                    CollectionAssert.AreEqual(this.Ints, this.View);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [TestCase(2, 1)]
        [TestCase(0, 2)]
        public void Replace(int index, int value)
        {
            using (var expected = this.Ints.SubscribeAll())
            {
                using (var actual = this.View.SubscribeAll())
                {
                    this.Ints[index] = value;
                    this.Scheduler?.Start();

                    Assert.AreEqual(value, this.View[index]);
                    CollectionAssert.AreEqual(this.Ints, this.View);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [TestCase(0, 1)]
        public void Move(int fromIndex, int toIndex)
        {
            using (var expected = this.Ints.SubscribeAll())
            {
                using (var actual = this.View.SubscribeAll())
                {
                    this.Ints.Move(fromIndex, toIndex);
                    this.Scheduler?.Start();

                    CollectionAssert.AreEqual(this.Ints, this.View);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }
    }
}
