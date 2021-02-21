namespace Gu.Reactive.Tests.Helpers
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Reactive;

    using NUnit.Framework;

    public static class EventPatternAssert
    {
        public static void AreEqual(object sender, string propertyName, EventPattern<PropertyChangedEventArgs> pattern)
        {
            Assert.AreSame(sender, pattern.Sender);
            Assert.AreEqual(propertyName, pattern.EventArgs.PropertyName);
        }

        [Obsolete("For testing obsolete API.")]
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
                Assert.AreEqual(default(T)!, pattern.EventArgs.Value);
            }
        }

        public static void AreEqual<TItem, TProperty>([AllowNull] TItem item, object? sender, object? source, Maybe<TProperty> value, string propertyName, EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>> actual)
        {
            Assert.AreSame(item, actual.EventArgs.Item);
            Assert.AreSame(sender, actual.Sender);
            Assert.AreSame(source, actual.EventArgs.SourceAndValue.Source);
            Assert.AreEqual(propertyName, actual.EventArgs.PropertyName);

            if (value.HasValue)
            {
                Assert.AreEqual(true, actual.EventArgs.SourceAndValue.Value.HasValue);
                Assert.AreEqual(value.Value, actual.EventArgs.Value);
                Assert.AreEqual(value, actual.EventArgs.SourceAndValue.Value);
            }
            else
            {
                Assert.AreEqual(false, actual.EventArgs.SourceAndValue.Value.HasValue);
                Assert.AreEqual(default(TProperty)!, actual.EventArgs.Value);
            }
        }
    }
}
