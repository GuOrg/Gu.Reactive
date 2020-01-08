namespace Gu.Wpf.Reactive.Tests
{
    using System.Threading.Tasks;

    using NUnit.Framework;

    public static class AssertCompletion
    {
        public static void AreEqual<T>(Task expected, NotifyTaskCompletionBase<T> actual)
            where T : Task
        {
            if (expected is null)
            {
                throw new System.ArgumentNullException(nameof(expected));
            }

            if (actual is null)
            {
                throw new System.ArgumentNullException(nameof(actual));
            }

            Assert.AreSame(expected, actual.Task);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.IsCompleted, actual.IsCompleted);
            Assert.AreEqual(expected.IsCanceled, actual.IsCanceled);
            Assert.AreEqual(expected.IsFaulted, actual.IsFaulted);
            var expectedException = expected.Exception;
            if (expectedException != null)
            {
                var actualException = actual.Exception;
                Assert.AreSame(expectedException.InnerException, actualException.InnerException);
            }

            Assert.AreEqual(expected.IsCanceled, actual.IsCanceled);
        }
    }
}
