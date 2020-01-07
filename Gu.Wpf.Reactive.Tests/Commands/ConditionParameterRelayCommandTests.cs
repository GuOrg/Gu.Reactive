namespace Gu.Wpf.Reactive.Tests
{
    using System.Threading.Tasks;
    using System.Windows;
    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;
    using Condition = Gu.Reactive.Condition;

    public class ConditionParameterRelayCommandTests
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
            var fake = new Fake { IsTrueOrNull = false };
            using (var condition = new Condition(fake.ObservePropertyChanged(x => x.IsTrueOrNull), () => fake.IsTrueOrNull))
            {
                using (var command = new ConditionRelayCommand<int>(x => { }, condition))
                {
                    command.CanExecuteChanged += (sender, args) => count++;
                    fake.IsTrueOrNull = true;
                    await Application.Current.Dispatcher.SimulateYield();
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