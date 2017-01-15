namespace Gu.Reactive.Tests.PropertyPathStuff
{
    using Gu.Reactive.PropertyPathStuff;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class PropertyPathTests
    {
        [Test]
        public void ToStringTest()
        {
            var path = PropertyPath.Create<Fake, int>(x => x.Next.Value);
            Assert.AreEqual("x => x.Next.Value", path.ToString());
        }
    }
}