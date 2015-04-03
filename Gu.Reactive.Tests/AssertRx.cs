namespace Gu.Reactive.Tests
{
    using System.ComponentModel;
    using System.Reactive;

    using NUnit.Framework;

    public static class AssertRx
    {
        public static void AreEqual(object sender, string propertyName, EventPattern<PropertyChangedEventArgs> pattern)
        {
            Assert.AreSame(sender, pattern.Sender);
            Assert.AreEqual(propertyName, pattern.EventArgs.PropertyName);
        }

        public static void AreEqual<TItem, TProperty>(object sender, string propertyName, TItem item, TProperty value, EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>> actual)
        {
            Assert.AreSame(sender, actual.Sender);
            Assert.AreEqual(propertyName, actual.EventArgs.PropertyName);

            Assert.AreEqual(value, actual.EventArgs.Value);
            Assert.AreSame(item, actual.EventArgs.Item);
        }
    }
}