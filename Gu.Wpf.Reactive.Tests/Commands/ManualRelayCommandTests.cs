using System;

namespace Gu.Wpf.Reactive.Tests
{
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;

    using NUnit.Framework;

    public class ManualRelayCommandTests
    {
        [Test(Description = "This is the most relevant test, it checks that the weak event implementation is correct")]
        public void MemoryLeak()
        {
            var command = new ManualRelayCommand(() => { }, () => true, false);
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
            var command = new ManualRelayCommand(() => { }, () => true, false);
            command.CanExecuteChanged += (sender, args) => count++;
            Assert.AreEqual(0, count);
            command.RaiseCanExecuteChanged();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ExecuteNotifies()
        {
            int invokeCount = 0;
            var command = new ManualRelayCommand(() => invokeCount++, () => true);
            command.Execute();
            Assert.AreEqual(1, invokeCount);
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
