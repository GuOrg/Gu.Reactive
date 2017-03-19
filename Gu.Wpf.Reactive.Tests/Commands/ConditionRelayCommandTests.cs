namespace Gu.Wpf.Reactive.Tests
{
    using System.Threading.Tasks;
    using System.Windows;
    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;
    using Condition = Gu.Reactive.Condition;

    public class ConditionRelayCommandTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            App.Start();
        }

        [Test]
        public async Task NotifiesOnConditionChanged()
        {
            var count = 0;
            var fake = new Fake { IsTrue = false };
            using (var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrue), () => fake.IsTrue))
            {
                using (var command = new ConditionRelayCommand(() => { }, condition))
                {
                    command.CanExecuteChanged += (sender, args) => count++;
                    Assert.AreEqual(0, count);
                    Assert.IsFalse(command.CanExecute());

                    fake.IsTrue = true;
                    await Application.Current.Dispatcher.SimulateYield();
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
            using (var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrue), () => fake.IsTrue))
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
            using (var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrue), () => fake.IsTrue))
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
