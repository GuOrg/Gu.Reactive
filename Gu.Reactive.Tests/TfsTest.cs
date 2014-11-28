namespace Gu.Reactive.Tests
{
    using System;
    using System.ComponentModel;
    using NUnit.Framework;

    public class TfsTest : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [Test]
        public void MemoryLeakTest()
        {
            var tfsTest = new TfsTest();
            PropertyChangedEventManager.AddHandler(tfsTest, Handler, "");
            var weakReference = new WeakReference(tfsTest);
            tfsTest = null;
            GC.Collect();
            Assert.IsFalse(weakReference.IsAlive);
        }

        private void Handler(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
        }
    }
}
