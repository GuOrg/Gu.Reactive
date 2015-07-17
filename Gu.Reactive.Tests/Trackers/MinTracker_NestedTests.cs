namespace Gu.Reactive.Tests.Trackers
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    using NUnit.Framework;

    public class MinTracker_NestedTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void InitializesWithValues(bool trackItemChanges)
        {
            var source = new ObservableCollection<Dummy>(new[] { new Dummy(1), new Dummy(2), new Dummy(3) });
            var tracker = source.TrackMin(x => x.Value, -1, trackItemChanges);
            Assert.AreEqual(1, tracker.Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void InitializesWhenEmpty(bool trackItemChanges)
        {
            var source = new ObservableCollection<Dummy>(new Dummy[0]);
            var tracker = source.TrackMin(x => x.Value, -1, trackItemChanges);
            Assert.AreEqual(-1, tracker.Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReactsAndNotifiesOnSourceCollectionChanges(bool trackItemChanges)
        {
            var source = new ObservableCollection<Dummy>(new[] { new Dummy(1), new Dummy(2), new Dummy(3) });
            var tracker = source.TrackMin(x => x.Value, -1, trackItemChanges);
            Assert.AreEqual(1, tracker.Value);
            int count = 0;
            tracker.ObservePropertyChanged(x => x.Value, false)
                   .Subscribe(_ => count++);
            source.RemoveAt(1);
            Assert.AreEqual(0, count);
            Assert.AreEqual(1, tracker.Value);

            source.RemoveAt(0);
            Assert.AreEqual(1, count);
            Assert.AreEqual(3, tracker.Value);

            source.RemoveAt(0);
            Assert.AreEqual(2, count);
            Assert.AreEqual(-1, tracker.Value);

            source.Add(new Dummy(4));
            Assert.AreEqual(3, count);
            Assert.AreEqual(4, tracker.Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReactsAndNotifiesOnItemChanges(bool trackItemChanges)
        {
            var source = new ObservableCollection<Dummy>(new[] { new Dummy(1), new Dummy(2), new Dummy(3) });
            var tracker = source.TrackMin(x => x.Value, -1, trackItemChanges);
            Assert.AreEqual(1, tracker.Value);
            int count = 0;
            tracker.ObservePropertyChanged(x => x.Value, false)
                   .Subscribe(_ => count++);
            source[1].Value = -3;
            if (trackItemChanges)
            {
                Assert.AreEqual(1, count);
                Assert.AreEqual(-3, tracker.Value);
            }
            else
            {
                Assert.AreEqual(0, count);
                Assert.AreEqual(1, tracker.Value);
            }
        }

        [Explicit("Longrunning benchmark")]
        [TestCase(1000, true)]
        [TestCase(1000, false)]
        public void Benchmark(int n, bool trackItemChanges)
        {
            var ints = new ObservableCollection<Dummy>();
            using (var tracker = ints.TrackMin(x => x.Value, -1, trackItemChanges))
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < n; i++)
                {
                    ints.Add(new Dummy(i));
                }
                sw.Stop();
                Console.WriteLine("Track item changes: {0}. {1} updates took {2} ms {3:F3} ms each", trackItemChanges, n, sw.ElapsedMilliseconds, sw.Elapsed.TotalMilliseconds / n);
            }
        }

        public class Dummy : INotifyPropertyChanged
        {
            private int _value;

            public Dummy(int value)
            {
                _value = value;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public int Value
            {
                get { return _value; }
                set
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}