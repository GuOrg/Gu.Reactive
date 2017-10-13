namespace Gu.Reactive.Tests.Trackers
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Tests.Helpers;
    using NUnit.Framework;

    public partial class MaxTrackerTests
    {
        public class Nested
        {
            [Test]
            public void InitializesWithValues()
            {
                var source = new ObservableCollection<Dummy> { new Dummy(1), new Dummy(2), new Dummy(3) };
                using (var tracker = source.TrackMax(x => x.Value))
                {
                    Assert.AreEqual(3, tracker.Value);
                }
            }

            [Test]
            public void InitializesWhenEmpty()
            {
                var source = new ObservableCollection<Dummy>(new Dummy[0]);
                using (var tracker = source.TrackMax(x => x.Value))
                {
                    Assert.AreEqual(null, tracker.Value);
                }
            }

            [Test]
            public void ReactsAndNotifiesOnSourceCollectionChanges()
            {
                var source = new ObservableCollection<Dummy> { new Dummy(1), new Dummy(2), new Dummy(3) };
                using (var tracker = source.TrackMax(x => x.Value))
                {
                    Assert.AreEqual(3, tracker.Value);
                    var count = 0;
                    using (tracker.ObservePropertyChanged(x => x.Value, signalInitial: false)
                                  .Subscribe(_ => count++))
                    {
                        source.RemoveAt(1);
                        Assert.AreEqual(0, count);
                        Assert.AreEqual(3, tracker.Value);

                        source.RemoveAt(1);
                        Assert.AreEqual(1, count);
                        Assert.AreEqual(1, tracker.Value);

                        source.RemoveAt(0);
                        Assert.AreEqual(2, count);
                        Assert.AreEqual(null, tracker.Value);

                        source.Add(new Dummy(4));
                        Assert.AreEqual(3, count);
                        Assert.AreEqual(4, tracker.Value);

                        source.Add(new Dummy(5));
                        Assert.AreEqual(4, count);
                        Assert.AreEqual(5, tracker.Value);

                        source[0].Value = 6;
                        Assert.AreEqual(5, count);
                        Assert.AreEqual(6, tracker.Value);
                    }
                }
            }

            [Test]
            public void ReactsAndNotifiesOnItemChangesOneLevel()
            {
                var source = new ObservableCollection<Dummy> { new Dummy(1), new Dummy(2), new Dummy(3) };
                using (var tracker = source.TrackMax(x => x.Value))
                {
                    Assert.AreEqual(3, tracker.Value);
                    var count = 0;
                    using (tracker.ObservePropertyChanged(x => x.Value, signalInitial: false)
                                  .Subscribe(_ => count++))
                    {
                        source[1].Value = 6;
                        Assert.AreEqual(1, count);
                        Assert.AreEqual(6, tracker.Value);
                    }
                }
            }

            [Test]
            public void ReactsAndNotifiesOnItemChangesTwoLevels()
            {
                var source = new ObservableCollection<Fake>
                {
                    new Fake { Level1 = new Level1 { Value = 1 } },
                    new Fake { Level1 = new Level1 { Value = 2 } },
                    new Fake { Level1 = new Level1 { Value = 3 } }
                };
                using (var tracker = source.TrackMax(x => x.Level1.Value))
                {
                    Assert.AreEqual(3, tracker.Value);
                    var count = 0;
                    using (tracker.ObservePropertyChanged(x => x.Value, signalInitial: false)
                                  .Subscribe(_ => count++))
                    {
                        source[2].Level1 = null;
                        Assert.AreEqual(1, count);
                        Assert.AreEqual(2, tracker.Value);

                        source[1].Level1 = null;
                        Assert.AreEqual(2, count);
                        Assert.AreEqual(1, tracker.Value);

                        source[0].Level1 = null;
                        Assert.AreEqual(3, count);
                        Assert.AreEqual(null, tracker.Value);

                        source[0].Level1 = new Level1 { Value = 3 };
                        Assert.AreEqual(4, count);
                        Assert.AreEqual(3, tracker.Value);

                        source[1].Level1 = new Level1 { Value = 2 };
                        Assert.AreEqual(4, count);
                        Assert.AreEqual(3, tracker.Value);
                    }
                }
            }

            public class Dummy : INotifyPropertyChanged
            {
                private int value;

                public Dummy(int value)
                {
                    this.value = value;
                }

                public event PropertyChangedEventHandler PropertyChanged;

                public int Value
                {
                    get => this.value;

                    set
                    {
                        if (value == this.value)
                        {
                            return;
                        }

                        this.value = value;
                        this.OnPropertyChanged();
                    }
                }

                public override string ToString()
                {
                    return $"{nameof(this.Value)}: {this.Value}";
                }

                protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}