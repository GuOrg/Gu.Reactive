namespace Gu.Reactive.Tests.PropertyPathStuff
{
    using System;

    using Gu.Reactive.PropertyPathStuff;
    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class PropertyPathParserTests
    {
        public bool IsTrue { get; }

        public Level Level { get; }

        public bool Method() => true;

        [Test]
        public void ThrowsOnMethod()
        {
            var exception = Assert.Throws<ArgumentException>(() => PropertyPathParser.GetPath<PropertyPathParserTests, bool>(x => x.Method()));
            Assert.AreEqual("Expected path to be properties only. Was x => x.Method()", exception.Message);
        }

        [Test]
        public void ThrowsOnNestedMethod()
        {
            var exception = Assert.Throws<ArgumentException>(() => PropertyPathParser.GetPath<Level, Level>(x => x.Next.Method()));
            Assert.AreEqual("Expected path to be properties only. Was x => x.Next.Method()", exception.Message);
        }

        [Test]
        public void GetPathOneLevel()
        {
            var actuals1 = PropertyPathParser.GetPath<Fake, bool>(x => x.IsTrue);
            CollectionAssert.AreEqual(new[] { typeof(Fake).GetProperty("IsTrue") }, actuals1);
            var actuals2 = PropertyPathParser.GetPath<Fake, bool>(f => f.IsTrue);
            Assert.AreSame(actuals1, actuals2);

            var fake = new Fake();
            var actuals3 = PropertyPathParser.GetPath(() => fake.IsTrue);
            Assert.AreSame(actuals1, actuals3);
        }

        [Test]
        public void GetPathsOneLevel()
        {
            var fake = new Fake();
            var fakeIsTrue = new[] { typeof(Fake).GetProperty("IsTrue") };
            CollectionAssert.AreEqual(fakeIsTrue, PropertyPathParser.GetPath<Fake, bool>(x => x.IsTrue));
            CollectionAssert.AreEqual(fakeIsTrue, PropertyPathParser.GetPath(() => fake.IsTrue));

            CollectionAssert.AreEqual(new[] { typeof(Fake).GetProperty("Name") }, PropertyPathParser.GetPath<Fake, string>(x => x.Name));
            CollectionAssert.AreEqual(new[] { typeof(PropertyPathParserTests).GetProperty("IsTrue") }, PropertyPathParser.GetPath<PropertyPathParserTests, bool>(x => x.IsTrue));
            CollectionAssert.AreEqual(fakeIsTrue, PropertyPathParser.GetPath<Fake, bool>(x => x.IsTrue));
            CollectionAssert.AreEqual(new[] { typeof(Fake).GetProperty("Name") }, PropertyPathParser.GetPath<Fake, string>(x => x.Name));
            CollectionAssert.AreEqual(new[] { typeof(PropertyPathParserTests).GetProperty("IsTrue") }, PropertyPathParser.GetPath<PropertyPathParserTests, bool>(x => x.IsTrue));
        }

        [Test]
        public void GetPathTwoLevels()
        {
            var actuals1 = PropertyPathParser.GetPath<Fake, bool>(x => x.Next.IsTrue);
            var expected = new[] { typeof(Fake).GetProperty("Next"), typeof(Level).GetProperty("IsTrue") };
            CollectionAssert.AreEqual(expected, actuals1);

            var actuals2 = PropertyPathParser.GetPath<Fake, bool>(f => f.Next.IsTrue);
            Assert.AreSame(actuals1, actuals2);

            var fake = new Fake();
            var actuals3 = PropertyPathParser.GetPath(() => fake.Next.IsTrue);
            Assert.AreSame(actuals1, actuals3);
        }

        [Test]
        public void GetPathTwoLevels2()
        {
            var actuals1 = PropertyPathParser.GetPath<Fake, bool>(x => x.Next.IsTrue);
            var expected1 = new[] { typeof(Fake).GetProperty("Next"), typeof(Level).GetProperty("IsTrue") };
            CollectionAssert.AreEqual(expected1, actuals1);

            var actuals2 = PropertyPathParser.GetPath<Fake, string>(fake => fake.Next.Name);
            var expected2 = new[] { typeof(Fake).GetProperty("Next"), typeof(Level).GetProperty("Name") };
            CollectionAssert.AreEqual(expected2, actuals2);
        }

        [Test]
        public void GetPathOneItemFuncOfTOneItemWithThis()
        {
            var properties1 = PropertyPathParser.GetPath(() => this.IsTrue);
            CollectionAssert.AreEqual(new[] { this.GetType().GetProperty("IsTrue") }, properties1);
            var properties2 = PropertyPathParser.GetPath(() => this.IsTrue);
            Assert.AreSame(properties1, properties2);
        }

        [Test]
        public void GetPathOneItemFuncOfTOneItemNoThis()
        {
#pragma warning disable SA1101 // Prefix local calls with this
            var properties1 = PropertyPathParser.GetPath(() => IsTrue);
#pragma warning restore SA1101 // Prefix local calls with this
            CollectionAssert.AreEqual(new[] { this.GetType().GetProperty("IsTrue") }, properties1);
            var properties2 = PropertyPathParser.GetPath(() => this.IsTrue);
            Assert.AreSame(properties1, properties2);
        }

        [Test]
        public void GetPathOneItem3()
        {
            var properties1 = PropertyPathParser.GetPath(() => this.IsTrue);
            CollectionAssert.AreEqual(new[] { this.GetType().GetProperty("IsTrue") }, properties1);
            var properties2 = PropertyPathParser.GetPath<PropertyPathParserTests, bool>(x => x.IsTrue);
            Assert.AreSame(properties1, properties2);
        }
    }
}
