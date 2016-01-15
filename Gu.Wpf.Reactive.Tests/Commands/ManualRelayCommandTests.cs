using System;

namespace Gu.Wpf.Reactive.Tests
{
    using Gu.Reactive;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;

    using NUnit.Framework;

    public class ManualRelayCommandTests
    {
        [Test(Description = "This is the most relevant test, it checks that the weak event implementation is correct")]
        public void MemoryLeak()
        {
            var command = new ManualRelayCommand(() => { }, () => true);
            var listener = new CommandListener();
            var wr = new WeakReference(listener);
            command.CanExecuteChanged += listener.React;
            listener = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            command.CanExecute(); // Touching it after to prevent GC
        }

        [Test]
        public void RaiseCanExecuteChanged()
        {
            int count = 0;
            var command = new ManualRelayCommand(() => { }, () => true);
            command.CanExecuteChanged += (sender, args) => count++;
            Assert.AreEqual(0, count);
            command.RaiseCanExecuteChanged();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ExecuteNotifies()
        {
            var invokeCount = 0;
            var isExecutingCount = 0;
            var command = new ManualRelayCommand(() => invokeCount++, () => true);
            command.ObservePropertyChangedSlim(nameof(command.IsExecuting), false)
                   .Subscribe(_ => isExecutingCount++);
            Assert.IsFalse(command.IsExecuting);
            Assert.True(command.CanExecute());
            command.Execute();
            Assert.IsFalse(command.IsExecuting);
            Assert.True(command.CanExecute());
            Assert.AreEqual(1, invokeCount);
            Assert.AreEqual(2, isExecutingCount);
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
