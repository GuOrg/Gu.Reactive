namespace Gu.Reactive.Tests.Helpers
{
    using System.Collections.Specialized;
    using System.ComponentModel;

    using NUnit.Framework;

    public static class AssertEx
    {
        public static void AreEqual(PropertyChangedEventArgs expected, object? actual)
        {
            if (expected is null &&
                actual is null)
            {
                return;
            }

            Assert.NotNull(expected);
            Assert.NotNull(actual);
            var actualChange = actual as PropertyChangedEventArgs;
            Assert.NotNull(actualChange, "Expected actual to be of type PropertyChangedEventArgs, was: " + actual!.GetType().Name);
            Assert.AreEqual(expected!.PropertyName, actualChange!.PropertyName);
        }

        public static void AreEqual(NotifyCollectionChangedEventArgs? expected, object? actual)
        {
            if (expected is null &&
                actual is null)
            {
                return;
            }

            Assert.NotNull(expected);
            Assert.NotNull(actual);
            var actualChange = actual as NotifyCollectionChangedEventArgs;
            Assert.NotNull(actualChange, "Expected actual to be of type NotifyCollectionChangedEventArgs, was: " + actual!.GetType().Name);

            Assert.AreEqual(expected!.Action, actualChange!.Action);

            Assert.AreEqual(expected.OldStartingIndex, actualChange.OldStartingIndex);
            CollectionAssert.AreEqual(expected.OldItems, actualChange.OldItems);

            Assert.AreEqual(expected.NewStartingIndex, actualChange.NewStartingIndex);
            CollectionAssert.AreEqual(expected.NewItems, actualChange.NewItems);
        }
    }
}
