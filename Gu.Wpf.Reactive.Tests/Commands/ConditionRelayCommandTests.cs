namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.ComponentModel;
    using System.Reactive;

    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ConditionRelayCommandTests
    {
        private Fake fake;

        private Condition condition;

        private ConditionRelayCommand command;

        private IObservable<EventPattern<PropertyChangedEventArgs>> observable;

        [SetUp]
        public void SetUp()
        {
            this.fake = new Fake { IsTrueOrNull = false };
            this.observable = this.fake.ObservePropertyChanged(x => x.IsTrueOrNull);
            this.condition = new Condition(this.observable, () => this.fake.IsTrueOrNull);
            this.command = new ConditionRelayCommand(() => { }, this.condition);
        }

        [Test]
        public void NotifiesOnConditionChanged()
        {
            int count = 0;
            var fake = new Fake { IsTrue = false };
            var observable = fake.ObservePropertyChanged(x => x.IsTrue);
            var condition = new Condition(observable, () => fake.IsTrue);
            var command = new ConditionRelayCommand(() => { }, condition);
            command.CanExecuteChanged += (sender, args) => count++;
            Assert.AreEqual(0, count);
            Assert.IsFalse(command.CanExecute());

            fake.IsTrue = true;
            Assert.AreEqual(1, count);
            Assert.IsTrue(command.CanExecute());
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CanExecute(bool expected)
        {
            this.fake.IsTrueOrNull = expected;
            Assert.AreEqual(expected, this.command.CanExecute());
        }

        [Test]
        public void Execute()
        {
            var i = 0;
            var command = new ConditionRelayCommand(() => i++, this.condition);
            command.Execute();
            Assert.AreEqual(1, i);
        }
    }
}
