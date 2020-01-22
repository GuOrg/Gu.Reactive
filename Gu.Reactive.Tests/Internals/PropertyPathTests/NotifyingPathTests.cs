namespace Gu.Reactive.Tests.Internals.PropertyPathTests
{
    using Gu.Reactive.Internals;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class NotifyingPathTests
    {
        [Test]
        public void ToStringTest()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var path = NotifyingPath.GetOrCreate<Fake, int>(x => x.Level1.Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.AreEqual("x => x.Level1.Value", path.ToString());

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            path = NotifyingPath.GetOrCreate<Fake, int>(x => x.Level1.Level2.Value);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.AreEqual("x => x.Level1.Level2.Value", path.ToString());
        }
    }
}
