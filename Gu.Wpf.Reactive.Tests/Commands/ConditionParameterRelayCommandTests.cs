namespace Gu.Wpf.Reactive.Tests
{
    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ConditionParameterRelayCommandTests
    {
        [Test]
        public void NotifiesOnConditionChanged()
        {
            int count = 0;
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull))
            {
                using (var command = new ConditionRelayCommand<int>(x => { }, condition))
                {
                    command.CanExecuteChanged += (sender, args) => count++;
                    fake.IsTrueOrNull = true;
                    Assert.AreEqual(1, count);
                }
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CanExecute(bool expected)
        {
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull))
            {
                using (var command = new ConditionRelayCommand<int>(x => { }, condition))
                {
                    fake.IsTrueOrNull = expected;
                    Assert.AreEqual(expected, command.CanExecute(0));
                }
            }
        }

        [Test]
        public void Execute()
        {
            var i = 0;
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull))
            {
                using (var command = new ConditionRelayCommand<int>(x => i = x, condition))
                {
                    command.Execute(1);
                    Assert.AreEqual(1, i);
                }
            }
        }
    }
}