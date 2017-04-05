namespace Gu.Reactive.Tests.Trackers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Tests.Helpers;

    using JetBrains.Annotations;

    using NUnit.Framework;

    public partial class MinMaxTrackerTests
    {
        public class Nested
        {
            [Test]
            public void InitializesWithValues()
            {
                var source = new ObservableCollection<Dummy> { new Dummy(1), new Dummy(2), new Dummy(3) };
                using (var tracker = source.TrackMinMax(x => x.Value))
                {
                    Assert.AreEqual(1, tracker.Min);
                    Assert.AreEqual(3, tracker.Max);
                }
            }

            [Test]
            public void InitializesWhenEmpty()
            {
                var source = new ObservableCollection<Dummy>(new Dummy[0]);
                using (var tracker = source.TrackMinMax(x => x.Value))
                {
                    Assert.AreEqual(null, tracker.Min);
                    Assert.AreEqual(null, tracker.Max);
                }
            }

            [Test]
            public void ReactsAndNotifiesOnSourceCollectionChanges()
            {
                var source = new ObservableCollection<Dummy> { new Dummy(1), new Dummy(2), new Dummy(3) };
                using (var tracker = source.TrackMinMax(x => x.Value))
                {
                    Assert.AreEqual(1, tracker.Min);
                    Assert.AreEqual(3, tracker.Max);
                    var changes = new List<string>();
                    using (tracker.ObservePropertyChangedSlim()
                                  .Subscribe(e => changes.Add(e.PropertyName)))
                    {
                        source.RemoveAt(1);
                        Assert.AreEqual(0, changes.Count);
                        Assert.AreEqual(1, tracker.Min);
                        Assert.AreEqual(3, tracker.Max);

                        source.RemoveAt(1);
                        var expected = new List<string> { "Max" };
                        CollectionAssert.AreEqual(expected, changes);
                        Assert.AreEqual(1, tracker.Min);
                        Assert.AreEqual(1, tracker.Max);

                        source.RemoveAt(0);
                        expected.AddRange(new[] { "Min", "Max" });
                        CollectionAssert.AreEqual(expected, changes);
                        Assert.AreEqual(null, tracker.Min);
                        Assert.AreEqual(null, tracker.Max);

                        source.Add(new Dummy(4));
                        expected.AddRange(new[] { "Min", "Max" });
                        CollectionAssert.AreEqual(expected, changes);
                        Assert.AreEqual(4, tracker.Min);
                        Assert.AreEqual(4, tracker.Max);

                        source.Add(new Dummy(5));
                        expected.Add("Max");
                        CollectionAssert.AreEqual(expected, changes);
                        Assert.AreEqual(4, tracker.Min);
                        Assert.AreEqual(5, tracker.Max);

                        source[0].Value = 6;
                        expected.AddRange(new[] { "Max", "Min" });
                        CollectionAssert.AreEqual(expected, changes);
                        Assert.AreEqual(5, tracker.Min);
                        Assert.AreEqual(6, tracker.Max);
                    }
                }
            }

            [Test]
            public void ReactsAndNotifiesOnItemChangesOneLevel()
            {
                var source = new ObservableCollection<Dummy> { new Dummy(1), new Dummy(2), new Dummy(3) };
                using (var tracker = source.TrackMinMax(x => x.Value))
                {
                    Assert.AreEqual(1, tracker.Min);
                    Assert.AreEqual(3, tracker.Max);
                    var changes = new List<string>();
                    using (tracker.ObservePropertyChangedSlim()
                                  .Subscribe(e => changes.Add(e.PropertyName)))
                    {
                        source[1].Value = 6;
                        CollectionAssert.AreEqual(new[] { "Max" }, changes);
                        Assert.AreEqual(1, tracker.Min);
                        Assert.AreEqual(6, tracker.Max);
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
                using (var tracker = source.TrackMinMax(x => x.Level1.Value))
                {
                    Assert.AreEqual(1, tracker.Min);
                    Assert.AreEqual(3, tracker.Max);
                    var changes = new List<string>();
                    using (tracker.ObservePropertyChangedSlim()
                                  .Subscribe(e => changes.Add(e.PropertyName)))
                    {
                        source[2].Level1 = null;
                        CollectionAssert.AreEqual(new[] { "Max" }, changes);
                        Assert.AreEqual(1, tracker.Min);
                        Assert.AreEqual(2, tracker.Max);

                        source[1].Level1 = null;
                        CollectionAssert.AreEqual(new[] { "Max", "Max" }, changes);
                        Assert.AreEqual(1, tracker.Min);
                        Assert.AreEqual(1, tracker.Max);

                        source[0].Level1 = null;
                        CollectionAssert.AreEqual(new[] { "Max", "Max", "Min", "Max" }, changes);
                        Assert.AreEqual(null, tracker.Min);
                        Assert.AreEqual(null, tracker.Max);

                        source[0].Level1 = new Level1 { Value = 3 };
                        CollectionAssert.AreEqual(new[] { "Max", "Max", "Min", "Max", "Min", "Max" }, changes);
                        Assert.AreEqual(3, tracker.Min);
                        Assert.AreEqual(3, tracker.Max);

                        source[1].Level1 = new Level1 { Value = 2 };
                        CollectionAssert.AreEqual(new[] { "Max", "Max", "Min", "Max", "Min", "Max", "Min" }, changes);
                        Assert.AreEqual(2, tracker.Min);
                        Assert.AreEqual(3, tracker.Max);
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

                [NotifyPropertyChangedInvocator]
                protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}