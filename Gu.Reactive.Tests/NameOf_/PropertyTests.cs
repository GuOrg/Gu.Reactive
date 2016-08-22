#pragma warning disable 618
namespace Gu.Reactive.Tests.NameOf_
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;


    // ReSharper disable once InconsistentNaming
    public class PropertyTests
    {
        public string StringProp { get; private set; }

        public Fake Fake { get; private set; }

        [Test]
        public void PropertyHappyPath()
        {
            var name = NameOf.Property(() => StringProp);
            Assert.AreEqual("StringProp", name);

            name = NameOf.Property<PropertyTests>(x => StringProp);
            Assert.AreEqual("StringProp", name);

            name = NameOf.Property<PropertyTests, string>(x => StringProp);
            Assert.AreEqual("StringProp", name);
        }

        [Test]
        public void PropertyHappyPath2()
        {
            var name = NameOf.Property(() => StringProp);
            Assert.AreEqual("StringProp", name);

            name = NameOf.Property<PropertyTests>(x => x.StringProp);
            Assert.AreEqual("StringProp", name);

            name = NameOf.Property<PropertyTests, string>(x => x.StringProp);
            Assert.AreEqual("StringProp", name);
        }

        [Test]
        public void BoxedPropertyHappyPath()
        {
            var fakeInpc = new Fake();
            var name = NameOf.Property(() => fakeInpc.IsTrue, true);
            Assert.AreEqual("IsTrue", name);

            name = NameOf.Property<Fake>(x => x.IsTrue);
            Assert.AreEqual("IsTrue", name);

            name = NameOf.Property<Fake, bool>(x => x.IsTrue);
            Assert.AreEqual("IsTrue", name);
        }

        [Test]
        public void NestedPropertyHappyPath()
        {
            var name = NameOf.Property(() => Fake.Next.Name, true);
            Assert.AreEqual("Name", name);

            name = NameOf.Property<PropertyTests>(x => Fake.Next.Name);
            Assert.AreEqual("Name", name);

            name = NameOf.Property<PropertyTests, string>(x => Fake.Next.Name);
            Assert.AreEqual("Name", name);
        }

        [Test]
        public void ThrowsOnNestedProperty()
        {
            var exception = Assert.Throws<ArgumentException>(() => NameOf.Property(() => StringProp.Length));
            Assert.AreEqual("Trying to get the name of a nested property: StringProp.Length", exception.Message);
        }

        [Test]
        public void ThrowsOnMethod()
        {
            Assert.Throws<ArgumentException>(() => NameOf.Property(() => Fake.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<PropertyTests>(x => x.Fake.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<PropertyTests, Level>(x => x.Fake.Method()));
        }

        [Test]
        public void ThrowsOnNestedMethod()
        {
            Assert.Throws<ArgumentException>(() => NameOf.Property(() => Fake.Next.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<PropertyTests>(x => x.Fake.Next.Method()));
            Assert.Throws<ArgumentException>(() => NameOf.Property<PropertyTests, Level>(x => x.Fake.Next.Method()));
        }

        [Test]
        public void OnNestedMethod()
        {
            var property = NameOf.Property(() => Fake.Method().Name, true);
            Assert.AreEqual("Name", property);

            property = NameOf.Property<PropertyTests>(x => Fake.Method().Name);
            Assert.AreEqual("Name", property);

            property = NameOf.Property<PropertyTests, string>(x => Fake.Method().Name);
            Assert.AreEqual("Name", property);
        }
    }
}
