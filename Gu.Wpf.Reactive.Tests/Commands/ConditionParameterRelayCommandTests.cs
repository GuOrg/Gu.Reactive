namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.ComponentModel;
    using System.Reactive;

    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ConditionParameterRelayCommandTests
    {
        private Fake fake;
        private Condition condition;
        private ConditionRelayCommand<int> command;
        private IObservable<EventPattern<PropertyChangedEventArgs>> observable;

        [SetUp]
        public void SetUp()
        {
            this.fake = new Fake { IsTrueOrNull = false };
            this.observable = this.fake.ObservePropertyChanged(x => x.IsTrueOrNull);
            this.condition = new Condition(this.observable, () => this.fake.IsTrueOrNull);
            this.command = new ConditionRelayCommand<int>(x => { }, this.condition);
        }

        [Test]
        public void NotifiesOnConditionChanged()
        {
            int count = 0;
            this.command.CanExecuteChanged += (sender, args) => count++;
            this.fake.IsTrueOrNull = true;
            Assert.AreEqual(1, count);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CanExecute(bool expected)
        {
            this.fake.IsTrueOrNull = expected;
            Assert.AreEqual(expected, this.command.CanExecute(0));
        }

        [Test]
        public void Execute()
        {
            var i = 0;
            using (var command = new ConditionRelayCommand<int>(x => i = x, this.condition))
            {
                command.Execute(1);
                Assert.AreEqual(1, i);
            }
        }
    }
}