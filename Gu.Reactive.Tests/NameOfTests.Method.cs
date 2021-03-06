﻿#pragma warning disable CS0618 // Type or member is obsolete
//// ReSharper disable once InconsistentNaming
namespace Gu.Reactive.Tests
{
    using NUnit.Framework;

    public static partial class NameOfTests
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

#pragma warning disable CA1822 // Mark members as static
            public int DummyMethod(int arg) => arg;

#pragma warning disable CA1801 // Review unused parameters
            public void DummyMethodVoid(int arg)
#pragma warning restore CA1801 // Review unused parameters
            {
            }
#pragma warning restore CA1822 // Mark members as static
        }
    }
}
