namespace Gu.Wpf.Reactive.Tests
{
    using Gu.Reactive;
    using Gu.Reactive.Tests;

    using NUnit.Framework;

    public class ConditionRelayCommandTests
    {
        [Test]
        public void NotifiesOnConditionChanged()
        {
            var fake = new FakeInpc { Prop1 = false };
            var observable = fake.ToObservable(x => x.Prop1);
            var condition = new Condition(observable, () => fake.Prop1);
            var command = new ConditionRelayCommand(_ => { }, condition, false);
            int count = 0;
            command.CanExecuteChanged += (sender, args) => count++;
            fake.Prop1 = true;
            Assert.AreEqual(1, count);
        }

        [Test]
        public void CanExecute()
        {
            var fake = new FakeInpc { Prop1 = false };
            var observable = fake.ToObservable(x => x.Prop1);
            var condition = new Condition(observable, () => fake.Prop1);
            var command = new ConditionRelayCommand(_ => { }, condition, false);
            Assert.IsFalse(command.CanExecute(null));

            fake.Prop1 = true;
            Assert.IsTrue(command.CanExecute(null));
        }
    }
}
