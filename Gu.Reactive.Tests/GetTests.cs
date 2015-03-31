namespace Gu.Reactive.Tests
{
    using System;

    using NUnit.Framework;

    public class GetTests
    {
        [Test]
        public void ValuePath()
        {
            this.Fake = new FakeInpc { Next = new Level { Name = "Johan" } };
            var path  = Get.ValuePath<GetTests,string>(x => x.Fake.Next.Name);
            Assert.IsTrue(path.HasValue(this));
            Assert.AreEqual("Johan", path.Value(this));
        }

        [Test]
        public void GetNameWhenNull()
        {
            var name = Get.ValueOrDefault(() => this.Fake.Next.Name);
            Assert.AreEqual(null, name);

            name = Get.ValueOrDefault(this, x => this.Fake.Next.Name);
            Assert.AreEqual(null, name);
        }

        [Test]
        public void GetNameWhenNotNull()
        {
            this.Fake = new FakeInpc { Next = new Level { Name = "Johan" } };
            var name = Get.ValueOrDefault(() => this.Fake.Next.Name);
            Assert.AreEqual("Johan", name);

            name = Get.ValueOrDefault(this, x => this.Fake.Next.Name);
            Assert.AreEqual("Johan", name);
        }

        [Test]
        public void GetNameWhenNullExplicit()
        {
            var name = Get.ValueOrDefault(() => this.Fake.Next.Name, "null");
            Assert.AreEqual("null", name);

            name = Get.ValueOrDefault(this, x => this.Fake.Next.Name, "null");
            Assert.AreEqual("null", name);
        }

        [Test]
        public void GetValueWhenNullDefault()
        {
            var value = Get.ValueOrDefault(() => this.Fake.Next.Value);
            Assert.AreEqual(0, value);
            value = Get.ValueOrDefault(this, x => this.Fake.Next.Value);
            Assert.AreEqual(int.MinValue, value);
        }

        [Test]
        public void GetValueWhenNullExplicit()
        {
            var value = Get.ValueOrDefault(() => this.Fake.Next.Value, int.MinValue);
            Assert.AreEqual(int.MinValue, value);

            value = Get.ValueOrDefault(this, x => this.Fake.Next.Value, int.MinValue);
            Assert.AreEqual(int.MinValue, value);
        }

        public FakeInpc Fake { get; private set; }
    }
}
