namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Reactive;
    using NUnit.Framework;

    public class DeferredViewTests
    {
        private ObservableCollection<int> _source;
        private TimeSpan _deferTime;
        private IDeferredView<int> _deferredView;
        private readonly List<NotifyCollectionChangedEventArgs> _changes = new List<NotifyCollectionChangedEventArgs>();

        [SetUp]
        public void SetUp()
        {
            _source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            _deferTime = TimeSpan.FromMilliseconds(10);
            _deferredView = _source.AsDeferredView(_deferTime);
            _changes.Clear();
        }

        [Test]
        public async Task ViewSignalsSameAsCollectionWhenCollectionIsChanged()
        {
            var collectionChanges = new List<EventArgs>();
            var viewChanges = new List<EventArgs>();
            var ints = new ObservableCollection<int>();
            ints.ObservePropertyChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            ints.ObserveCollectionChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            var view = ints.AsDeferredView(TimeSpan.FromMilliseconds(10));
            view.ObservePropertyChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            view.ObserveCollectionChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            ints.Add(1);
            await Task.Delay(20);
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(collectionChanges, viewChanges, new EventArgsComparer());
        }

        [Test]
        public void BenchMark()
        {
            var ints = new ObservableCollection<int>();
            var view = ints.AsDeferredView(TimeSpan.FromMilliseconds(10));
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                ints.Add(i);
            }
            Console.WriteLine("adding 1000 took: {0} ms", sw.ElapsedMilliseconds);

            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                view.Add(i);
            }
            Console.WriteLine("adding 1000 took: {0} ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public async Task ViewSignalsSameAsCollectionWhenViewIsChanged()
        {
            var collectionChanges = new List<EventArgs>();
            var viewChanges = new List<EventArgs>();
            var ints = new ObservableCollection<int>();
            ints.ObservePropertyChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            ints.ObserveCollectionChanged().Subscribe(x => collectionChanges.Add(x.EventArgs));
            var view = ints.AsDeferredView(TimeSpan.FromMilliseconds(10));
            view.ObservePropertyChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            view.ObserveCollectionChanged().Subscribe(x => viewChanges.Add(x.EventArgs));
            view.Add(1);
            await Task.Delay(20);
            CollectionAssert.AreEqual(ints, view);
            CollectionAssert.AreEqual(collectionChanges, viewChanges, new EventArgsComparer());
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
            await Task.Delay(40);
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
