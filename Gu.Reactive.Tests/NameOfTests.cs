namespace Gu.Reactive.Tests
{
    using System;
    using System.Linq;

    using NUnit.Framework;
    public class NameOfTests
    {
        public string DummyProperty { get; private set; }

        [Test]
        public void NameOfPropertyHappyPath()
        {
            var name = NameOf.Property(() => DummyProperty);
            Assert.AreEqual("DummyProperty", name);
        }
        [Test]
        public void ThrowsOnNestedProperty()
        {
            var exception = Assert.Throws<Exception>(() => NameOf.Property(() => DummyProperty.Length));
        }

        [Test]
        public void NameOfMethodAction()
        {
            var actual = NameOf.Method(() => DummyMethod(0));
            Assert.AreEqual("DummyMethod", actual);
        }

        [Test]
        public void NameOfMethodFunc()
        {
            var actual = NameOf.Method(() => DummyMethod(0));
            Assert.AreEqual("DummyMethod", actual);
        }

        [Test]
        public void ArgumentsAction()
        {
            int i = 0;
            var actual = NameOf.Arguments(() => DummyMethodVoid(i));
            CollectionAssert.AreEqual(new[] { "arg" }, actual.Select(x => x.Name));
            CollectionAssert.AreEqual(new[] { 0 }, actual.Select(x => x.Value));
        }

        [Test]
        public void ArgumentsFunc()
        {
            int i = 0;
            var actual = NameOf.Arguments(() => DummyMethod(i));
            CollectionAssert.AreEqual(new[] { "arg" }, actual.Select(x => x.Name));
            CollectionAssert.AreEqual(new[] { 0 }, actual.Select(x => x.Value));
        }

        public int DummyMethod(int arg)
        {
            return arg;
        }

        public void DummyMethodVoid(int arg)
        {
        }
    }
}
