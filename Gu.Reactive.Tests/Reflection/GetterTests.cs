namespace Gu.Reactive.Tests.Reflection
{
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class GetterTests
    {
        [Test]
        public void Caching()
        {
            var getter1 = Getter.GetOrCreate(typeof(Fake).GetProperty("Name"));
            var getter2 = Getter.GetOrCreate(typeof(Fake).GetProperty("Name"));
            Assert.AreSame(getter1, getter2);

            var getter3 = Getter.GetOrCreate(typeof(Level).GetProperty("Name"));
            var getter4 = Getter.GetOrCreate(typeof(Level).GetProperty("Name"));
            Assert.AreSame(getter3, getter4);

            Assert.AreNotSame(getter1, getter3);
        }

        [Test]
        public void GetValue()
        {
            var source = new Fake { Name = "meh" };
            var getter = Getter.GetOrCreate(typeof(Fake).GetProperty("Name"));
            Assert.AreEqual("meh", getter.GetValue(source));
            var genericGetter = (Getter<Fake, string>)getter;
            Assert.AreEqual("meh", genericGetter.GetValue(source));
        }

        [Test]
        public void GetValueStruct()
        {
            var source = new StructLevel { Name = "meh" };
            var getter = Getter.GetOrCreate(typeof(StructLevel).GetProperty("Name"));
            Assert.AreEqual("meh", getter.GetValue(source));
            var genericGetter = (Getter<StructLevel, string>)getter;
            Assert.AreEqual("meh", genericGetter.GetValue(source));
        }

        [Test]
        public void GetValueGeneric()
        {
            var source = new Fake<int> { Value = 1 };
            var getter = Getter.GetOrCreate(typeof(Fake<int>).GetProperty("Value"));
            Assert.AreEqual(1, getter.GetValue(source));
            var genericGetter = (Getter<Fake<int>, int>)getter;
            Assert.AreEqual(1, genericGetter.GetValue(source));
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