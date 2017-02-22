namespace Gu.Reactive.Tests.Internals.PropertyPathTests
{
    using Gu.Reactive.Internals;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class PropertyPathTests
    {
        [Test]
        public void ToStringTest()
        {
            var path = PropertyPath.GetOrCreate<Fake, int>(x => x.Next.Value);
            Assert.AreEqual("x => x.Next.Value", path.ToString());
        }
    }
}