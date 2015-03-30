namespace Gu.Reactive.Tests
{
    using System.Linq;

    using NUnit.Framework;

    // ReSharper disable once InconsistentNaming
    public class NameOf_Method_Tests
    {
        [Test]
        public void MethodAction()
        {
            var actual = NameOf.Method(() => DummyMethod(0));
            Assert.AreEqual("DummyMethod", actual);
        }

        [Test]
        public void MethodFunc()
        {
            var actual = NameOf.Method<NameOf_Method_Tests>(x => DummyMethod(0));
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