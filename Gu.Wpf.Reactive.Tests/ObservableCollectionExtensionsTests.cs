namespace Gu.Wpf.Reactive.Tests
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public class ObservableCollectionExtensionsTests
    {
        [Test]
        public async Task RemoveAsyncTest()
        {
            var ints = new ObservableCollection<int> { 1 };
            Assert.IsFalse(await ints.RemoveAsync(2));
            CollectionAssert.AreEqual(new[] { 1 }, ints);
            Assert.IsTrue(await ints.RemoveAsync(1));
            CollectionAssert.IsEmpty(ints);

        }
    }
}
