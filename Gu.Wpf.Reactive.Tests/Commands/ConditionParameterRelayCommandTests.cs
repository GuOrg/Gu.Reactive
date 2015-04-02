namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.ComponentModel;
    using System.Reactive;

    using Gu.Reactive;
    using Gu.Reactive.Tests;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ConditionParameterRelayCommandTests
    {
        private Fake _fake;
        private Condition _condition;
        private ConditionRelayCommand<int> _command;
        private IObservable<EventPattern<PropertyChangedEventArgs>> _observable;

        [SetUp]
        public void SetUp()
        {
            _fake = new Fake { IsTrueOrNull = false };
            _observable = _fake.ObservePropertyChanged(x => x.IsTrueOrNull);
            _condition = new Condition(_observable, () => _fake.IsTrueOrNull);
            _command = new ConditionRelayCommand<int>(x => { }, _condition);
        }
        [Test]
        public void NotifiesOnConditionChanged()
        {
            int count = 0;
            _command.CanExecuteChanged += (sender, args) => count++;
            _fake.IsTrueOrNull = true;
            Assert.AreEqual(1, count);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CanExecute(bool expected)
        {
            _fake.IsTrueOrNull = expected;
            Assert.AreEqual(expected, _command.CanExecute(0));
        }

        [Test]
        public void Execute()
        {
            var i = 0;
            var command = new ConditionRelayCommand<int>(x => i=x, _condition);
            command.Execute(1);
            Assert.AreEqual(1, i);
        }
    }
}