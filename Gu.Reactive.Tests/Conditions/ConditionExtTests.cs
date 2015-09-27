namespace Gu.Reactive.Tests.Conditions
{
    using System;
    using System.Reactive.Subjects;

    using NUnit.Framework;

    public class ConditionExtTests
    {
        [Test, Explicit("Not sure if we want caching")]
        public void Caches()
        {
            var observable = new Subject<object>();
            var condition = new Condition(observable, () => true);
            var o1 = condition.ObserveIsSatisfied();
            var o2 = condition.ObserveIsSatisfied();
            Assert.AreSame(o1, o2);
        }

        [Test]
        public void Signals()
        {
            var source = new Subject<object>();
            var isSatisfied = false;
            var condition = new Condition(source, () => isSatisfied);
            ICondition result = null;
            var o1 = condition.ObserveIsSatisfied()
                              .Subscribe(x => result = x);
            isSatisfied = true;
            source.OnNext(null);
            Assert.AreSame(condition, result);
        }
    }
}