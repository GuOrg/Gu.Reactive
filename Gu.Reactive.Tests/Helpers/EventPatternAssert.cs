namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;
    using System.Reactive;

    using NUnit.Framework;

    public static class EventPatternAssert
    {
        public static void AreEqual(object sender, string propertyName, EventPattern<PropertyChangedEventArgs> pattern)
        {
            Assert.AreSame(sender, pattern.Sender);
            Assert.AreEqual(propertyName, pattern.EventArgs.PropertyName);
        }

        public static void AreEqual<T>(object sender, string propertyName, Maybe<T> value, EventPattern<PropertyChangedAndValueEventArgs<T>> pattern)
        {
            Assert.AreSame(sender, pattern.Sender);
            Assert.AreEqual(propertyName, pattern.EventArgs.PropertyName);
            if (value.HasValue)
            {
                Assert.AreEqual(true, pattern.EventArgs.HasValue);
                Assert.AreEqual(value.Value, pattern.EventArgs.Value);
            }
            else
            {
                Assert.AreEqual(false, pattern.EventArgs.HasValue);
                Assert.AreEqual(default(T), pattern.EventArgs.Value);
            }
        }

        public static void AreEqual<TItem, TProperty>(TItem item, object sender, object source, Maybe<TProperty> value, string propertyName, EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>> actual)
        {
            Assert.AreSame(sender, actual.Sender);
            Assert.AreSame(item, actual.EventArgs.Item);
            Assert.AreSame(source, actual.EventArgs.ValueSource);
            Assert.AreEqual(propertyName, actual.EventArgs.PropertyName);

            if (value.HasValue)
            {
                Assert.AreEqual(value.Value, actual.EventArgs.Value);
                Assert.AreEqual(true, actual.EventArgs.HasValue);
            }
            else
            {
                Assert.AreEqual(default(TProperty), actual.EventArgs.Value);
                Assert.AreEqual(false, actual.EventArgs.HasValue);
            }
        }
    }
}