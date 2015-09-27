namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.ComponentModel;
    using System.Reactive;

    using Gu.Reactive;
    using Gu.Reactive.Tests;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ConditionRelayCommandTests
    {
        private Fake _fake;

        private Condition _condition;

        private ConditionRelayCommand _command;

        private IObservable<EventPattern<PropertyChangedEventArgs>> _observable;

        [SetUp]
        public void SetUp()
        {
            _fake = new Fake { IsTrueOrNull = false };
            _observable = _fake.ObservePropertyChanged(x => x.IsTrueOrNull);
            _condition = new Condition(_observable, () => _fake.IsTrueOrNull);
            _command = new ConditionRelayCommand(() => { }, _condition);
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
            _fake.IsTrueOrNull = expected;
            Assert.AreEqual(expected, _command.CanExecute());
        }

        [Test]
        public void Execute()
        {
            var i = 0;
            var command = new ConditionRelayCommand(() => i++, _condition);
            command.Execute();
            Assert.AreEqual(1, i);
        }
    }
}
