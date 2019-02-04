namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Gu.Reactive;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;

    public class ManualRelayCommandTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            App.Start();
        }

        [Test(Description = "This is the most relevant test, it checks that the weak event implementation is correct")]
        public void MemoryLeak()
        {
#if DEBUG
            return; // debugger keeps things alive.
#endif
#pragma warning disable CS0162 // Unreachable code detected
            var command = new ManualRelayCommand(() => { }, () => true);
#pragma warning restore CS0162 // Unreachable code detected
            var listener = new CommandListener();
            var wr = new WeakReference(listener);
            command.CanExecuteChanged += listener.React;
            //// ReSharper disable once RedundantAssignment
            listener = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            _ = command.CanExecute(); // Touching it after to prevent GC
        }

        [Test]
        public async Task RaiseCanExecuteChanged()
        {
            var count = 0;
            var command = new ManualRelayCommand(() => { }, () => true);
            command.CanExecuteChanged += (sender, args) => count++;
            Assert.AreEqual(0, count);
            command.RaiseCanExecuteChanged();
            await Application.Current.Dispatcher.SimulateYield();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ExecuteNotifies()
        {
            var invokeCount = 0;
            var isExecutingCount = 0;
            var command = new ManualRelayCommand(() => invokeCount++, () => true);
            using (command.ObservePropertyChangedSlim(nameof(command.IsExecuting), signalInitial: false)
                          .Subscribe(_ => isExecutingCount++))
            {
                Assert.IsFalse(command.IsExecuting);
                Assert.True(command.CanExecute());
                command.Execute();
                Assert.IsFalse(command.IsExecuting);
                Assert.True(command.CanExecute());
                Assert.AreEqual(1, invokeCount);
                Assert.AreEqual(2, isExecutingCount);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CanExecute(bool canExecute)
        {
            var command = new ManualRelayCommand(() => { }, () => canExecute);
            Assert.AreEqual(canExecute, command.CanExecute());
        }
    }
}
