namespace Gu.Reactive.Tests.PropertyPathStuff
{
    using Gu.Reactive.PropertyPathStuff;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class GetterTests
    {
        [Test]
        public void GetValue()
        {
            var fake = new Fake { Name = "meh" };
            var getter = Getter.GetOrCreate(typeof(Fake).GetProperty("Name"));
            Assert.AreEqual("meh", getter.GetValue(fake));
        }

        [Test]
        public void GetValueGeneric()
        {
            var fake = new Fake { Name = "meh" };
            var getter = (Getter<Fake, string>)Getter.GetOrCreate(typeof(Fake).GetProperty("Name"));
            Assert.AreEqual("meh", getter.GetValue(fake));
        }

        [Test]
        public void GetValueViaDelegate()
        {
            var fake = new Fake { Name = "meh" };
            var propertyInfo = typeof(Fake).GetProperty("Name");
            Assert.AreEqual("meh", propertyInfo.GetValueViaDelegate(fake));
        }
    }
}