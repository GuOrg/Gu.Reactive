namespace Gu.Reactive.Tests
{
    using System;

    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class NameOf_Property_Tests
    {
        public string StringProp { get; private set; }

        public FakeInpc Fake { get; private set; }

        [Test]
        public void PropertyHappyPath()
        {
            var name = NameOf.Property(() => StringProp);
            Assert.AreEqual("StringProp", name);

            name = NameOf.Property<NameOf_Property_Tests>(x => StringProp);
            Assert.AreEqual("StringProp", name);

            name = NameOf.Property<NameOf_Property_Tests, string>(x => StringProp);
            Assert.AreEqual("StringProp", name);
        }

        [Test]
        public void NestedPropertyHappyPath()
        {
            var name = NameOf.Property(() => Fake.Next.Name, true);
            Assert.AreEqual("Name", name);

            name = NameOf.Property<NameOf_Property_Tests>(x => Fake.Next.Name);
            Assert.AreEqual("Name", name);

            name = NameOf.Property<NameOf_Property_Tests, string>(x => Fake.Next.Name);
            Assert.AreEqual("Name", name);
        }

        [Test]
        public void ThrowsOnNestedProperty()
        {
            var exception = Assert.Throws<ArgumentException>(() => NameOf.Property(() => StringProp.Length));
        }

        [Test]
        public void ThrowsOnMethod()
        {
            Assert.Throws<ArgumentException>(() => NameOf.Property(() => Fake.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<NameOf_Property_Tests>(x => x.Fake.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<NameOf_Property_Tests, Level>(x => x.Fake.Method()));
        }

        [Test]
        public void ThrowsOnNestedMethod()
        {
            Assert.Throws<ArgumentException>(() => NameOf.Property(() => Fake.Next.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<NameOf_Property_Tests>(x => x.Fake.Next.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<NameOf_Property_Tests, Level>(x => x.Fake.Next.Method()));
        }

        [Test]
        public void OnNestedMethod()
        {
            var property = NameOf.Property(() => Fake.Method().Name, true);
            Assert.AreEqual("Name", property);

            property = NameOf.Property<NameOf_Property_Tests>(x => Fake.Method().Name);
            Assert.AreEqual("Name", property);

            property = NameOf.Property<NameOf_Property_Tests, string>(x => Fake.Method().Name);
            Assert.AreEqual("Name", property);
        }
    }
}
