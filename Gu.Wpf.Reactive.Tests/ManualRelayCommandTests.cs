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
            var command = new ManualRelayCommand(_ => { }, _ => true, false);
            var listener = new CommandListener();
            var wr = new WeakReference(listener);
            command.CanExecuteChanged += listener.React;
            listener = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
            command.CanExecute(null); // Touching it after to prevent GC
        }

        [Test]
        public void RaiseCanExecuteChanged()
        {
            int count = 0;
            var command = new ManualRelayCommand(_ => { }, _ => true, false);
            command.CanExecuteChanged += (sender, args) => count++;
            Assert.AreEqual(0, count);
            command.RaiseCanExecuteChanged();
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ExecuteNoParameter()
        {
            int invokeCount = 0;
            var command = new ManualRelayCommand(_ => invokeCount++, _ => true);
            command.Execute(null);
            Assert.AreEqual(1, invokeCount);
        }

        [Test]
        public void ExecuteWithParameter()
        {
            int invokeCount = 0;
            var command = new ManualRelayCommand(o => invokeCount = (int)o, _ => true);
            command.Execute(4);
            Assert.AreEqual(4, invokeCount);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CanExecute(bool canExecute)
        {
            var command = new ManualRelayCommand(_ => { }, _ => canExecute);
            Assert.AreEqual(canExecute, command.CanExecute(null));
        }

        [Test]
        public void CanExecuteWithParameter()
        {
            int i = 5;
            var command = new ManualRelayCommand(_ => { }, o => i == (int)o);
            Assert.IsTrue(command.CanExecute(5));
            Assert.IsFalse(command.CanExecute(4));
        }
    }
}
