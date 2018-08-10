namespace Gu.Reactive.Tests.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Gu.Reactive.Internals;
    using Gu.Reactive.Tests.Helpers;
    using Moq;
    using NUnit.Framework;

    public class PropertyPathParserTests
    {
        public bool IsTrue { get; } = true;

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
        public void GetPathFromMock()
        {
            var fake = Mock.Of<IReadOnlyObservableCollection<int>>();
            var actuals = PropertyPathParser.GetPath(() => fake.Count);
            var expected = typeof(IReadOnlyCollection<int>).GetProperty("Count");
            Assert.NotNull(expected);
            Assert.AreEqual(1, actuals.Count);
            Assert.AreEqual(expected.Name, actuals[0].Name);
            Assert.AreEqual(expected.PropertyType, actuals[0].PropertyType);
            Assert.AreEqual(expected.DeclaringType, actuals[0].DeclaringType);
            Assert.AreEqual(expected.GetMethod, actuals[0].GetMethod);
            Assert.AreEqual(typeof(IReadOnlyObservableCollection<int>), actuals[0].ReflectedType);
        }

        [Test]
        public void ReadOnlyObservableCollectionCount()
        {
            var actuals1 = PropertyPathParser.GetPath<ReadOnlyObservableCollection<int>, int>(x => x.Count);
            CollectionAssert.AreEqual(new[] { typeof(ReadOnlyObservableCollection<int>).GetProperty("Count") }, actuals1);
            var actuals2 = PropertyPathParser.GetPath<ReadOnlyObservableCollection<int>, int>(f => f.Count);
            Assert.AreSame(actuals1, actuals2);

            var ints = new ReadOnlyObservableCollection<int>(new ObservableCollection<int>());
            var actuals3 = PropertyPathParser.GetPath(() => ints.Count);
            Assert.AreSame(actuals1, actuals3);
        }

        [Test]
        public void ReadOnlyCollectionAndReadOnlyObservableCollectionCount()
        {
            var actuals1 = PropertyPathParser.GetPath<ReadOnlyObservableCollection<int>, int>(x => x.Count);
            CollectionAssert.AreEqual(new[] { typeof(ReadOnlyObservableCollection<int>).GetProperty("Count") }, actuals1);
            var actuals2 = PropertyPathParser.GetPath<ReadOnlyCollection<int>, int>(f => f.Count);
            CollectionAssert.AreEqual(new[] { typeof(ReadOnlyCollection<int>).GetProperty("Count") }, actuals2);
        }

        [Test]
        public void FakeOfReadOnlyObservableCollectionCount()
        {
            var actuals1 = PropertyPathParser.GetPath<Fake<ReadOnlyObservableCollection<int>>, int>(x => x.Value.Count);
            var expected = new[]
            {
                typeof(Fake<ReadOnlyObservableCollection<int>>).GetProperty("Value"),
                typeof(ReadOnlyObservableCollection<int>).GetProperty("Count"),
            };
            CollectionAssert.AreEqual(expected, actuals1);
            var actuals2 = PropertyPathParser.GetPath<Fake<ReadOnlyObservableCollection<int>>, int>(f => f.Value.Count);
            Assert.AreSame(actuals1, actuals2);

            var ints = new Fake<ReadOnlyObservableCollection<int>>();
            var actuals3 = PropertyPathParser.GetPath(() => ints.Value.Count);
            Assert.AreSame(actuals1, actuals3);
        }

        [Test]
        public void GetPathOneLevelGeneric()
        {
            var actuals1 = PropertyPathParser.GetPath<Level<int>, int>(x => x.Value);
            CollectionAssert.AreEqual(new[] { typeof(Level<int>).GetProperty("Value") }, actuals1);
            var actuals2 = PropertyPathParser.GetPath<Level<int>, int>(f => f.Value);
            Assert.AreSame(actuals1, actuals2);

            var fake = new Level<int>();
            var actuals3 = PropertyPathParser.GetPath(() => fake.Value);
            Assert.AreSame(actuals1, actuals3);
        }

        [Test]
        public void GetPathOneLevelGenerics()
        {
            var intActuals1 = PropertyPathParser.GetPath<Level<int>, int>(x => x.Value);
            CollectionAssert.AreEqual(new[] { typeof(Level<int>).GetProperty("Value") }, intActuals1);
            var doubleActuals1 = PropertyPathParser.GetPath<Level<double>, double>(x => x.Value);
            CollectionAssert.AreEqual(new[] { typeof(Level<double>).GetProperty("Value") }, doubleActuals1);
            Assert.AreNotSame(intActuals1, doubleActuals1);

            var intLevel = new Level<int>();
            var intActuals2 = PropertyPathParser.GetPath(() => intLevel.Value);
            Assert.AreSame(intActuals1, intActuals2);

            var doubleLevel = new Level<double>();
            var doubleActuals2 = PropertyPathParser.GetPath(() => doubleLevel.Value);
            Assert.AreSame(doubleActuals1, doubleActuals2);
        }

        [Test]
        public void GetPathOneAndTwoLevels()
        {
            var actuals1 = PropertyPathParser.GetPath<Fake, Level>(x => x.Next);
            CollectionAssert.AreEqual(new[] { typeof(Fake).GetProperty("Next") }, actuals1);

            var actuals2 = PropertyPathParser.GetPath<Fake, int>(f => f.Next.Value1);
            CollectionAssert.AreEqual(new[] { typeof(Fake).GetProperty("Next"), typeof(Level).GetProperty("Value1") }, actuals2);

            var actuals3 = PropertyPathParser.GetPath<Fake, int>(f => f.Next.Value2);
            CollectionAssert.AreEqual(new[] { typeof(Fake).GetProperty("Next"), typeof(Level).GetProperty("Value2") }, actuals3);
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
        public void GetPathTwoLevelsGeneric()
        {
            var actuals1 = PropertyPathParser.GetPath<Fake<int>, int>(x => x.Next.Value);
            var expected = new[] { typeof(Fake<int>).GetProperty("Next"), typeof(Level<int>).GetProperty("Value") };
            CollectionAssert.AreEqual(expected, actuals1);

            var actuals2 = PropertyPathParser.GetPath<Fake<int>, int>(x => x.Next.Value);
            Assert.AreSame(actuals1, actuals2);

            var fake = new Fake<int>();
            var actuals3 = PropertyPathParser.GetPath(() => fake.Next.Value);
            Assert.AreSame(actuals1, actuals3);
        }

        [Test]
        public void GetPathTwoLevelsGenerics()
        {
            var intActuals1 = PropertyPathParser.GetPath<Fake<int>, int>(x => x.Next.Value);
            var intExpecteds = new[] { typeof(Fake<int>).GetProperty("Next"), typeof(Level<int>).GetProperty("Value") };
            CollectionAssert.AreEqual(intExpecteds, intActuals1);

            var intActuals2 = PropertyPathParser.GetPath<Fake<int>, int>(x => x.Next.Value);
            Assert.AreSame(intActuals1, intActuals2);

            var intFake = new Fake<int>();
            var intActuals3 = PropertyPathParser.GetPath(() => intFake.Next.Value);
            Assert.AreSame(intActuals1, intActuals3);

            var doubleActuals1 = PropertyPathParser.GetPath<Fake<double>, double>(x => x.Next.Value);
            var doubleExpecteds = new[] { typeof(Fake<double>).GetProperty("Next"), typeof(Level<double>).GetProperty("Value") };
            CollectionAssert.AreEqual(doubleExpecteds, doubleActuals1);

            var doubleActuals2 = PropertyPathParser.GetPath<Fake<double>, double>(x => x.Next.Value);
            Assert.AreSame(doubleActuals1, doubleActuals2);
            Assert.AreNotSame(intActuals1, doubleActuals2);

            var doubleFake = new Fake<double>();
            var doubleActuals3 = PropertyPathParser.GetPath(() => doubleFake.Next.Value);
            Assert.AreSame(doubleActuals1, doubleActuals3);
            Assert.AreNotSame(intActuals1, doubleActuals2);
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
