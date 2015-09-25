// ReSharper disable All
namespace Gu.Reactive.Tests.Sandbox
{
    using System;

    using NUnit.Framework;

    [Explicit("Sandbox")]
    public class GC_BoxTests
    {
        [Test]
        public void GCCollect_B()
        {
            var b = new B { A = new A() }; // For some reason using object initializer prevents GC
            var wr = new WeakReference(b);
            b = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void GCCollect_B2()
        {
            var b = new B(new A());
            var wr = new WeakReference(b);
            b = null;
            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }

        [Test]
        public void GCCollect_B3()
        {
            WeakReference wr = new WeakReference(null);
            new Action(
                () =>
                {
                    var b = new B { A = new A() };
                    wr.Target = b;
                })();

            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }


        [Test]
        public void GCCollect_B4()
        {
            WeakReference wr = new WeakReference(null);
            // Testing a scope :)
            {
                var b = new B { A = new A() };
                wr.Target = b;
            }

            GC.Collect();
            Assert.IsFalse(wr.IsAlive);
        }
    }


    public class A { }

    public class B
    {
        public B() { }

        public B(A a)
        {
            A = a;
        }
        public A A { get; set; }
    }
}
