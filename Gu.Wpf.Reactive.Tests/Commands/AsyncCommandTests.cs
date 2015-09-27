namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gu.Reactive;

    using Moq;

    using NUnit.Framework;

    public class AsyncCommandTests
    {
        [Test]
        public void CanExecuteNoCondition()
        {
            var command = new AsyncCommand(() => Task.FromResult(1));
            Assert.IsTrue(command.CanExecute());
            Assert.IsInstanceOf<Condition>(command.Condition);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CanExecuteCondition(bool expected)
        {
            var condition = Mock.Of<ICondition>(x => x.IsSatisfied == expected);
            var command = new AsyncCommand(() => Task.FromResult(1), condition);
            Assert.AreEqual(2, ((AndCondition)command.Condition).Prerequisites.Count);
            Assert.AreEqual(expected, command.CanExecute());
        }

        [Test]
        public async Task ExecuteNotifiesCanExecuteChanged()
        {
            var count = 0;
            var tcs = new TaskCompletionSource<int>();
            var command = new AsyncCommand(() => tcs.Task);
            command.CanExecuteChanged += (_, __) => count++;
            command.Execute();
            Assert.AreEqual(1, count);
            tcs.SetResult(1);
            await command.Execution.Task;
            Assert.AreEqual(2, count);
        }

        [Test]
        public async Task ExecuteFinished()
        {
            var finished = Task.FromResult(1);
            var command = new AsyncCommand(() => finished);
            Assert.IsTrue(command.CanExecute());
            command.Execute();
            await command.Execution.Task;
            Assert.IsTrue(command.CanExecute());
            Assert.AreSame(finished, command.Execution.Task);
            Assert.AreSame(finished, command.Execution.Completed);
        }

        [Test]
        public void ExecuteCanceled()
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetCanceled();
            var command = new AsyncCommand(() => tcs.Task);
            command.Execute();
            Assert.AreSame(tcs.Task, command.Execution.Task);
        }

        [Test]
        public async Task ExecuteThrows()
        {
            var exception = new Exception();
            var command = new AsyncCommand(() => Task.Run(() => { throw exception; }));
            command.Execute();
            try
            {
                await command.Execution.Task;
            }
            catch
            {
            }
            Assert.AreEqual(exception, command.Execution.InnerException);
            Assert.AreEqual(TaskStatus.Faulted, command.Execution.Status);
            Assert.AreEqual(true, command.CanExecute());
        }

        [Test]
        public async Task CannotExecuteWhileRunning()
        {
            var resetEvent = new ManualResetEventSlim();
            var command = new AsyncCommand(() => Task.Run(() => resetEvent.Wait()));
            Assert.IsTrue(command.CanExecute());
            command.Execute();
            Assert.IsFalse(command.CanExecute());
            resetEvent.Set();
            await command.Execution.Task;
            Assert.IsTrue(command.CanExecute());
        }
    }
}
