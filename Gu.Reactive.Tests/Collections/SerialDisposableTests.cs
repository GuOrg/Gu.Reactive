namespace Gu.Reactive.Tests.Collections
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
                mock.Setup(x => x.Dispose());
            }

            mock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void DisposesOnAssignWhenDisposed()
        {
            using (var serialDisposable = new SerialDisposable<IDisposable>())
            {
                serialDisposable.Dispose();
                var mock = new Mock<IDisposable>(MockBehavior.Strict);
                mock.Setup(x => x.Dispose());
                serialDisposable.Disposable = mock.Object;
                mock.Verify(x => x.Dispose(), Times.Once);
            }
        }
    }
}