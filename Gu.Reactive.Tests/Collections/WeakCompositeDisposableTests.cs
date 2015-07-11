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
            var compositeDisposable = new WeakCompositeDisposable();
            var mock1 = new Mock<IDisposable>();
            var mock2 = new Mock<IDisposable>();
            compositeDisposable.Add(mock1.Object);
            compositeDisposable.Add(mock2.Object);
            compositeDisposable.Dispose();
            mock1.Verify(x => x.Dispose(), Times.Once);
            mock2.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
