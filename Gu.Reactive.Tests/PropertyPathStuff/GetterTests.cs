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
            var source = new Fake { Name = "meh" };
            var getter = Getter.GetOrCreate(typeof(Fake).GetProperty("Name"));
            Assert.AreEqual("meh", getter.GetValue(source));
        }

        [Test]
        public void GetValueStruct()
        {
            var source = new StructLevel { Name = "meh" };
            var getter = Getter.GetOrCreate(typeof(StructLevel).GetProperty("Name"));
            Assert.AreEqual("meh", getter.GetValue(source));
        }

        [Test]
        public void GetValueGeneric()
        {
            var source = new Fake { Name = "meh" };
            var getter = (Getter<Fake, string>)Getter.GetOrCreate(typeof(Fake).GetProperty("Name"));
            Assert.AreEqual("meh", getter.GetValue(source));
        }

        [Test]
        public void GetValueGenericStruct()
        {
            var source = new StructLevel { Name = "meh" };
            var getter = (Getter<StructLevel, string>)Getter.GetOrCreate(typeof(StructLevel).GetProperty("Name"));
            Assert.AreEqual("meh", getter.GetValue(source));
        }

        [Test]
        public void GetValueViaDelegate()
        {
            var source = new Fake { Name = "meh" };
            var propertyInfo = typeof(Fake).GetProperty("Name");
            Assert.AreEqual("meh", propertyInfo.GetValueViaDelegate(source));
        }
    }
}