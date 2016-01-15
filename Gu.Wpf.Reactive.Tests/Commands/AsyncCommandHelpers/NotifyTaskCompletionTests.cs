namespace Gu.Wpf.Reactive.Tests.AsyncCommandHelpers
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public class NotifyTaskCompletionTests
    {
        [Test]
        public void ResultFromFinishedTypedTask()
        {
            var fromResult = Task.FromResult(1);
            var completion = new NotifyTaskCompletion<int>(fromResult);
            Assert.AreEqual(fromResult.Result, completion.Completed.Result);
            AssertCompletion.AreEqual(fromResult, completion);
        }

        [Test]
        public async Task AwaitTypedTask()
        {
            var tcs = new TaskCompletionSource<int>();
            var task = tcs.Task;
            var completion = new NotifyTaskCompletion<int>(task);
            tcs.SetResult(1);
            await completion.Task.ConfigureAwait(false);
            Assert.AreEqual(task.Result, completion.Completed.Result);
            AssertCompletion.AreEqual(task, completion);
        }

        [Test]
        public void CancelTypedTask()
        {
            var tcs = new TaskCompletionSource<int>();
            var task = tcs.Task;
            var completion = new NotifyTaskCompletion<int>(task);
            tcs.SetCanceled();
            //Assert.AreEqual(task.Result, completion.Result);
            AssertCompletion.AreEqual(task, completion);
        }

        [Test]
        public void ThrowTypedTask()
        {
            var tcs = new TaskCompletionSource<int>();
            var task = tcs.Task;
            var completion = new NotifyTaskCompletion<int>(task);
            tcs.SetException(new Exception());
            AssertCompletion.AreEqual(task, completion);
        }

        [Test]
        public void ResultFromFinishedTask()
        {
            var fromResult = Task.FromResult(1);
            var completion = new NotifyTaskCompletion(fromResult);
            AssertCompletion.AreEqual(fromResult, completion);
        }

        [Test]
        public void AwaitTask()
        {
            var tcs = new TaskCompletionSource<int>();
            var task = tcs.Task;
            var completion = new NotifyTaskCompletion(task);
            tcs.SetResult(1);
            AssertCompletion.AreEqual(task, completion);
        }

        [Test]
        public void CancelTask()
        {
            var tcs = new TaskCompletionSource<int>();
            var task = tcs.Task;
            var completion = new NotifyTaskCompletion(task);
            tcs.SetCanceled();
            AssertCompletion.AreEqual(task, completion);
        }

        [Test]
        public void ThrowTask()
        {
            var tcs = new TaskCompletionSource<int>();
            var task = tcs.Task;
            var completion = new NotifyTaskCompletion(task);
            tcs.SetException(new Exception());
            AssertCompletion.AreEqual(task, completion);
        }
    }
}
