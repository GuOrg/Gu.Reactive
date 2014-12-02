namespace Gu.Wpf.Reactive.Tests
{
    using System;
    using System.Threading.Tasks;

    using NUnit.Framework;

    public class AsyncCommandTests
    {
        private TaskCompletionSource<int> _tcs;
        private Task<int> _task;
        private AsyncCommand _command;

        [SetUp]
        public void SetUp()
        {
            _tcs = new TaskCompletionSource<int>();
            _task = _tcs.Task;
            _command = new AsyncCommand(() => _task);
        }

        [Test]
        public void CanExecuteNoCondition()
        {
            Assert.IsTrue(_command.CanExecute());
        }

        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        public void CanExecuteCondition(int i, int value, bool expected)
        {
            var command = new AsyncCommand(() => _task, () => value == i);
            Assert.AreEqual(expected, command.CanExecute());
        }

        [Test]
        public void ExecuteNotifiesCanExecuteChanged()
        {
            var n = 0;
            _command.CanExecuteChanged += (_, __) => n++;
            _command.Execute();
            Assert.AreEqual(1, n);
            _tcs.SetResult(1);
            Assert.AreEqual(2, n);
        }

        [Test]
        public void ExecuteFinished()
        {
            _command.Execute();
            AssertCompletion.AreEqual(_task, _command.Execution);
        }

        [Test]
        public void ExecuteCanceled()
        {
            _tcs.SetCanceled();
            _command.Execute();
            AssertCompletion.AreEqual(_task, _command.Execution);
        }

        [Test]
        public void ExecuteThrow()
        {
            _tcs.SetException(new Exception());
            _command.Execute();
            AssertCompletion.AreEqual(_task, _command.Execution);
        }

        [Test]
        public void BlocksMultiple()
        {
            Assert.IsTrue(_command.CanExecute());
            _command.Execute();
            Assert.IsFalse(_command.CanExecute());
            _tcs.SetResult(0);
        }
    }
}
