namespace Gu.Reactive.Tests.Get
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    using Get = Gu.Reactive.Get;

    public class ValuePathTests
    {
        public Fake Fake { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Fake = null;
        }

        [Test]
        public void ValuePathWhenHasValue()
        {
            Fake = new Fake { Next = new Level { Name = "Johan" } };
            var path = Get.ValuePath<ValuePathTests, string>(x => x.Fake.Next.Name);
            var value = path.GetValue(this);
            Assert.IsTrue(value.HasValue);
            Assert.AreEqual("Johan", value.Value);
        }

        [Test]
        public void ValuePathWhenHasNullValue()
        {
            Fake = new Fake { Next = new Level { Name = null } };
            var path = Get.ValuePath<ValuePathTests, string>(x => x.Fake.Next.Name);
            var value = path.GetValue(this);
            Assert.IsTrue(value.HasValue);
            Assert.AreEqual(null, value.Value);
        }

        [Test]
        public void ValuePathWhenNullInPath()
        {
            Fake = new Fake();
            var path = Get.ValuePath<ValuePathTests, string>(x => x.Fake.Next.Name);
            var value = path.GetValue(this);
            Assert.IsFalse(value.HasValue);
            // ReSharper disable once UnusedVariable
            Assert.Throws<InvalidOperationException>(()=> { var temp = value.Value; });
        }
    }
}
