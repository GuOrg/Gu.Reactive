namespace Gu.Reactive.Tests
{
    using System;
    using Moq;

    using NUnit.Framework;

    public class SerialDisposableTests
    {
        [Test]
        public void Disposes()
        {
            var mock = new Mock<IDisposable>(MockBehavior.Strict);
            using (var serialDisposable = new SerialDisposable<IDisposable>())
            {
                serialDisposable.Disposable = mock.Object;
                Assert.AreSame(mock.Object, serialDisposable.Disposable);
                mock.Setup(x => x.Dispose());
            }

            mock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void DisposesOnAssignWhenDisposed()
        {
            using var serialDisposable = new SerialDisposable<IDisposable>();
#pragma warning disable IDISP016, IDISP017 // Don't use disposed instance.
            serialDisposable.Dispose();
#pragma warning restore IDISP016, IDISP017 // Don't use disposed instance.
            var mock = new Mock<IDisposable>(MockBehavior.Strict);
            mock.Setup(x => x.Dispose());
            serialDisposable.Disposable = mock.Object;
            mock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void AssignSameTwiceDoesNotDispose()
        {
            using var serialDisposable = new SerialDisposable<IDisposable>();
            var mock = new Mock<IDisposable>(MockBehavior.Strict);
            serialDisposable.Disposable = mock.Object;
            serialDisposable.Disposable = mock.Object;
            mock.Setup(x => x.Dispose());
        }
    }
}
