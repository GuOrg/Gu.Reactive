namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using System.Threading;
    using System.Threading.Tasks;

    using Gu.Reactive;

    using Moq;

    using NUnit.Framework;

    public class AsyncParameterCommandTests
    {
        [Test]
        public void CanExecuteNoCondition()
        {
            var command = new AsyncCommand<int>(x => Task.FromResult(1));
            Assert.IsTrue(command.CanExecute(0));
            Assert.IsFalse(command.CancelCommand.CanExecute());
            Assert.IsInstanceOf<Condition>(command.Condition);
        }

        [Test]
        public void CanCancel()
        {
            var command = new AsyncCommand<int>((i,x) => x.AsObservable().FirstAsync().ToTask());
            Assert.IsTrue(command.CanExecute(0));
            Assert.IsFalse(command.CancelCommand.CanExecute());
            command.Execute(0);
            Assert.IsTrue(command.CancelCommand.CanExecute());
            command.CancelCommand.Execute();
            Assert.IsFalse(command.CancelCommand.CanExecute());
            Assert.IsInstanceOf<Condition>(command.Condition);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CanExecuteCondition(bool expected)
        {
            var condition = Mock.Of<ICondition>(x => x.IsSatisfied == expected);
            var command = new AsyncCommand<int>(x => Task.FromResult(1), condition);
            Assert.AreEqual(2, ((AndCondition)command.Condition).Prerequisites.Count);
            Assert.AreEqual(expected, command.CanExecute(0));
        }

        [Test]
        public async Task ExecuteNotifiesCanExecuteChanged()
        {
            var count = 0;
            var tcs = new TaskCompletionSource<int>();
            var command = new AsyncCommand<int>(x => tcs.Task);
            command.CanExecuteChanged += (_, __) => count++;
            Assert.AreEqual(0, count);
            Assert.IsTrue(command.CanExecute(0));
            Assert.IsFalse(command.CancelCommand.CanExecute());
            command.Execute(0);
            Assert.IsFalse(command.CancelCommand.CanExecute());
            Assert.AreEqual(1, count);
            tcs.SetResult(1);
            await command.Execution.Task;
            Assert.AreEqual(2, count);
        }

        [Test]
        public async Task ExecuteFinished()
        {
            var finished = Task.FromResult(1);
            var command = new AsyncCommand<int>(x => finished);
            Assert.IsTrue(command.CanExecute(0));
            command.Execute(0);
            await command.Execution.Task;
            Assert.IsTrue(command.CanExecute(0));
            Assert.AreSame(finished, command.Execution.Task);
            Assert.AreSame(finished, command.Execution.Completed);
        }

        [Test]
        public void ExecuteCanceled()
        {
            var tcs = new TaskCompletionSource<int>();
            tcs.SetCanceled();
            var command = new AsyncCommand<int>(x => tcs.Task);
            command.Execute(0);
            Assert.AreSame(tcs.Task, command.Execution.Task);
        }

        [Test]
        public async Task ExecuteThrows()
        {
            var exception = new Exception();
            var command = new AsyncCommand<int>(x => Task.Run(() => { throw exception; }));
            command.Execute(0);
            try
            {
                await command.Execution.Task;
            }
            catch
            {
            }
            Assert.AreEqual(exception, command.Execution.InnerException);
            Assert.AreEqual(TaskStatus.Faulted, command.Execution.Status);
            Assert.AreEqual(true, command.CanExecute(0));
        }

        [Test]
        public async Task CannotExecuteWhileRunning()
        {
            var resetEvent = new ManualResetEventSlim();
            var command = new AsyncCommand<int>(x => Task.Run(() => resetEvent.Wait()));
            Assert.IsTrue(command.CanExecute(0));
            command.Execute(0);
            Assert.IsFalse(command.CanExecute(0));
            resetEvent.Set();
            await command.Execution.Task;
            Assert.IsTrue(command.CanExecute(0));
        }
    }
}