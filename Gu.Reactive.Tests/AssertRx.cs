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

        public static void AreEqual<TItem, TSender, TProperty>(string propertyName, TItem item, TSender sender, TProperty value, EventPattern<ItemPropertyChangedEventArgs<TSender, string>> actual)
        {
            Assert.AreSame(sender, actual.Sender);

            Assert.AreEqual(item, actual.EventArgs.Item);
            Assert.AreEqual(propertyName, actual.EventArgs.PropertyName);
            Assert.AreEqual(value, actual.EventArgs.Value);
        }
    }
}