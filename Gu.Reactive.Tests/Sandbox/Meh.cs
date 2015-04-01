namespace Gu.Reactive.Tests.Sandbox
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using NUnit.Framework;

    public class Meh
    {
        [Test]
        public void TestNameTest()
        {
            var a = 1*2 + 3/4;
        }

        public class C<TSource, TItem>
            where TSource : IEnumerable<TItem>, INotifyCollectionChanged
            where TItem : INotifyPropertyChanged
        {

        }
    }
}
