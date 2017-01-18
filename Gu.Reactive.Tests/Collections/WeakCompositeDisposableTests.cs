namespace Gu.Reactive.Tests.Collections
{
    using System;
    using Moq;
    using NUnit.Framework;

    public class WeakCompositeDisposableTests
    {
        [Test]
        public void Disposes()
        {
            Mock<IDisposable> mock1 = new Mock<IDisposable>(MockBehavior.Strict);
            Mock<IDisposable> mock2 = new Mock<IDisposable>(MockBehavior.Strict);
            using (var compositeDisposable = new WeakCompositeDisposable())
            {
                compositeDisposable.Add(mock1.Object);
                compositeDisposable.Add(mock2.Object);
                mock1.Verify(x => x.Dispose(), Times.Never);
                mock2.Verify(x => x.Dispose(), Times.Never);
                mock1.Setup(x => x.Dispose());
                mock2.Setup(x => x.Dispose());
            }

            mock1.Verify(x => x.Dispose(), Times.Once);
            mock2.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void DisposesOnAddWhenDisposed()
        {
            using (var compositeDisposable = new WeakCompositeDisposable())
            {
                compositeDisposable.Dispose();
                var mock = new Mock<IDisposable>(MockBehavior.Strict);
                mock.Setup(x => x.Dispose());
                compositeDisposable.Add(mock.Object);
                mock.Verify(x => x.Dispose(), Times.Once);
            }
        }
    }
}
