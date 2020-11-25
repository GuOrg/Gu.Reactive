namespace Gu.Wpf.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    using NUnit.Framework;

    [Obsolete("Testing obsolete API.")]
    public static class ObservableCollectionExtensionsTests
    {
        [Test]
        public static void InvokeInsertSorted()
        {
            var ints = new ObservableCollection<int> { 1 };
            ints.InvokeInsertSorted(2, Comparer<int>.Default.Compare);
            CollectionAssert.AreEqual(new[] { 1, 2 }, ints);
            ints.InvokeInsertSorted(0, Comparer<int>.Default.Compare);
            CollectionAssert.AreEqual(new[] { 0, 1, 2 }, ints);
            ints.InvokeInsertSorted(1, Comparer<int>.Default.Compare);
            CollectionAssert.AreEqual(new[] { 0, 1, 1, 2 }, ints);
        }

        [Test]
        public static void InvokeAdd()
        {
            var ints = new ObservableCollection<int> { 1 };
            ints.InvokeAdd(2);
            CollectionAssert.AreEqual(new[] { 1, 2 }, ints);
        }

        [Test]
        public static void InvokeAddRange()
        {
            var ints = new ObservableCollection<int> { 1 };
            ints.InvokeAddRange(new[] { 2, 3 });
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, ints);
        }

        [Test]
        public static void InvokeRemove()
        {
            var ints = new ObservableCollection<int> { 1, 2 };
            ints.InvokeRemove(2);
            CollectionAssert.AreEqual(new[] { 1 }, ints);
        }

        [Test]
        public static void InvokeRemoveRange()
        {
            var ints = new ObservableCollection<int> { 1, 2, 3 };
            ints.InvokeRemoveRange(new[] { 2, 3 });
            CollectionAssert.AreEqual(new[] { 1 }, ints);
        }

        [Test]
        public static void InvokeClear()
        {
            var ints = new ObservableCollection<int> { 1 };
            ints.InvokeClear();
            CollectionAssert.IsEmpty(ints);
        }

        [Test]
        public static async Task AddAsync()
        {
            var ints = new ObservableCollection<int> { 1 };
            await ints.AddAsync(2).ConfigureAwait(false);
            CollectionAssert.AreEqual(new[] { 1, 2 }, ints);
        }

        [Test]
        public static async Task AddRangeAsync()
        {
            var ints = new ObservableCollection<int> { 1 };
            await ints.AddRangeAsync(new[] { 2, 3 }).ConfigureAwait(false);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, ints);
        }

        [Test]
        public static async Task RemoveAsyncTest()
        {
            var ints = new ObservableCollection<int> { 1 };
            Assert.IsFalse(await ints.RemoveAsync(2).ConfigureAwait(false));
            CollectionAssert.AreEqual(new[] { 1 }, ints);
            Assert.IsTrue(await ints.RemoveAsync(1).ConfigureAwait(false));
            CollectionAssert.IsEmpty(ints);
        }

        [Test]
        public static async Task ClearAsync()
        {
            var ints = new ObservableCollection<int> { 1 };
            await ints.ClearAsync().ConfigureAwait(false);
            CollectionAssert.IsEmpty(ints);
        }
    }
}
