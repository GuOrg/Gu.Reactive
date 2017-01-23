namespace Gu.Wpf.Reactive.Tests
{
    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ConditionRelayCommandTests
    {
        [Test]
        public void NotifiesOnConditionChanged()
        {
            int count = 0;
            var fake = new Fake { IsTrue = false };
            var observable = fake.ObservePropertyChanged(x => x.IsTrue);
            using (var condition = new Condition(observable, () => fake.IsTrue))
            {
                using (var command = new ConditionRelayCommand(() => { }, condition))
                {
                    command.CanExecuteChanged += (sender, args) => count++;
                    Assert.AreEqual(0, count);
                    Assert.IsFalse(command.CanExecute());

                    fake.IsTrue = true;
                    Assert.AreEqual(1, count);
                    Assert.IsTrue(command.CanExecute());
                }
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CanExecute(bool expected)
        {
            var fake = new Fake { IsTrue = false };
            var observable = fake.ObservePropertyChanged(x => x.IsTrue);
            using (var condition = new Condition(observable, () => fake.IsTrue))
            {
                using (var command = new ConditionRelayCommand(() => { }, condition))
                {
                    fake.IsTrue = expected;
                    Assert.AreEqual(expected, command.CanExecute());
                }
            }
        }

        [Test]
        public void Execute()
        {
            var fake = new Fake { IsTrue = true };
            var observable = fake.ObservePropertyChanged(x => x.IsTrue);
            using (var condition = new Condition(observable, () => fake.IsTrue))
            {
                var i = 0;
                using (var command = new ConditionRelayCommand(() => i++, condition))
                {
                    command.Execute();
                    Assert.AreEqual(1, i);
                }
            }
        }
    }
}
