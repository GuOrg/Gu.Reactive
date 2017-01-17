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
            Mock<IDisposable> mock1;
            Mock<IDisposable> mock2;
            using (var compositeDisposable = new WeakCompositeDisposable())
            {
                mock1 = new Mock<IDisposable>();
                mock2 = new Mock<IDisposable>();
                compositeDisposable.Add(mock1.Object);
                compositeDisposable.Add(mock2.Object);
                mock1.Verify(x => x.Dispose(), Times.Never);
                mock2.Verify(x => x.Dispose(), Times.Never);
            }

            mock1.Verify(x => x.Dispose(), Times.Once);
            mock2.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
