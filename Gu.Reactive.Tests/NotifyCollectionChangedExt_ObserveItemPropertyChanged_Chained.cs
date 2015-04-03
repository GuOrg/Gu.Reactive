namespace Gu.Reactive.Tests
{
    using System.Collections.Generic;
    using System.Reactive;

    using Gu.Reactive.Tests.Fakes;

    using NUnit.Framework;

    public class NotifyCollectionChangedExt_ObserveItemPropertyChanged_Chained
    {
        private List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<EventPattern<ItemPropertyChangedEventArgs<Fake, string>>>();
        }
    }
}