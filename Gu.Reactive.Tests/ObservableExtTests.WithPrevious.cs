namespace Gu.Reactive.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;
    using NUnit.Framework;

    public partial class ObservableExtTests
    {
        public class WithPrevious
        {
            [Test]
            public void Sequence()
            {
                using (var subject = new Subject<int>())
                {
                    var actuals = new List<WithPrevious<int>>();
                    using (subject.WithPrevious().Subscribe(x => actuals.Add(x)))
                    {
                        CollectionAssert.IsEmpty(actuals);

                        subject.OnNext(1);
                        CollectionAssert.IsEmpty(actuals);

                        subject.OnNext(2);
                        CollectionAssert.AreEqual(new[] { "1,2" }, actuals.Select(x => $"{x.Previous},{x.Current}"));

                        subject.OnNext(3);
                        CollectionAssert.AreEqual(new[] { "1,2", "2,3" }, actuals.Select(x => $"{x.Previous},{x.Current}"));
                    }
                }
            }
        }
    }
}