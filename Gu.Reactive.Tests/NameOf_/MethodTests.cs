namespace Gu.Reactive.Tests.NameOf_
{
    using System.Linq;

    using NUnit.Framework;

    #pragma warning disable CS0618 // Type or member is obsolete
    // ReSharper disable once InconsistentNaming
    public class MethodTests
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
            var actual = NameOf.Method<MethodTests>(x => DummyMethod(0));
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
    #pragma warning restore CS0618 // Type or member is obsolete
}