#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS0619 // Type or member is obsolete
//// ReSharper disable once InconsistentNaming
namespace Gu.Reactive.Tests
{
    using NUnit.Framework;

    public partial class NameOfTests
    {
        public class Method
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
                var actual = NameOf.Method<Method>(x => this.DummyMethod(0));
                Assert.AreEqual(nameof(this.DummyMethod), actual);
            }

            [Test]
            public void MethodFunc2()
            {
                var actual = NameOf.Method<Method, int>(x => this.DummyMethod(0));
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

}
