#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS0619 // Type or member is obsolete
namespace Gu.Reactive.Tests.NameOf_
{
    using System.Linq;

    using NUnit.Framework;

    //// ReSharper disable once InconsistentNaming
    public class MethodTests
    {
        [Test]
        public void MethodAction1()
        {
            var actual = NameOf.Method(() => this.DummyMethodVoid(0));
            Assert.AreEqual(nameof(this.DummyMethodVoid), actual);
        }

        [Test]
        public void MethodAction2()
        {
            var actual = NameOf.Method(() => this.DummyMethod(0));
            Assert.AreEqual(nameof(this.DummyMethod), actual);
        }

        [Test]
        public void MethodFunc1()
        {
            var actual = NameOf.Method<MethodTests>(x => this.DummyMethod(0));
            Assert.AreEqual(nameof(this.DummyMethod), actual);
        }

        [Test]
        public void MethodFunc2()
        {
            var actual = NameOf.Method<MethodTests, int>(x => this.DummyMethod(0));
            Assert.AreEqual(nameof(this.DummyMethod), actual);
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