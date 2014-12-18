namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public class DeferredCollectionTests
    {
        private ObservableCollection<int> _source;
        private TimeSpan _deferTime;
        private DeferredView<int> _deferredView;
        private readonly List<NotifyCollectionChangedEventArgs> _changes = new List<NotifyCollectionChangedEventArgs>();
        [SetUp]
        public void SetUp()
        {
            _source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            _deferTime = TimeSpan.FromMilliseconds(10);
            _deferredView = new DeferredView<int>(_source, _deferTime);
            _changes.Clear();
        }

        [Test]
        public async Task OneChangeOneNotification()
        {
            _deferredView.CollectionChanged += (_, e) => _changes.Add(e);
            _source.Add(4);
            await Task.Delay(50);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, _changes.Single().Action);
        }

        [Test]
        public async Task ManyChangeOneReset()
        {
            _deferredView.CollectionChanged += (_, e) => _changes.Add(e);
            for (int i = 0; i < 10; i++)
            {
                _source.Add(i);
            }
            await Task.Delay(50);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, _changes.Single().Action);
        }

        [Test]
        public async Task TwoBurstsTwoResets()
        {
            _deferredView.CollectionChanged += (_, e) => _changes.Add(e);
            for (int i = 0; i < 10; i++)
            {
                _source.Add(i);
            }
            await Task.Delay(20);
            for (int i = 0; i < 10; i++)
            {
                _source.Add(i);
            }
            await Task.Delay(50);
            Assert.AreEqual(2, _changes.Count);
            Assert.IsTrue(_changes.All(x => x.Action == NotifyCollectionChangedAction.Reset));
        }
    }
}
