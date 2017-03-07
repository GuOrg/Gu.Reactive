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
            var path = PropertyPath.GetOrCreate<Fake, int>(x => x.Level1.Value);
            Assert.AreEqual("x => x.Level1.Value", path.ToString());

            path = PropertyPath.GetOrCreate<Fake, int>(x => x.Level1.Level2.Value);
            Assert.AreEqual("x => x.Level1.Level2.Value", path.ToString());
        }
    }
}