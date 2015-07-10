namespace Gu.Reactive.Tests.Get
{
    using Gu.Reactive.Tests.Fakes;

    using NUnit.Framework;

    using Get = Gu.Reactive.Get;

    public class ValueOrDefaultTests
    {
        public Fake Fake { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Fake = null;
        }

        [Test]
        public void GetNameWhenNull()
        {
            var name = Get.ValueOrDefault(() => this.Fake.Next.Name);
            Assert.AreEqual(null, name);

            name = Get.ValueOrDefault(this, x => this.Fake.Next.Name);
            Assert.AreEqual(null, name);
        }

        [Test, Explicit("Implement this?")]
        public void GetWithMethod()
        {
            var name = Get.ValueOrDefault(() => this.Fake.Method().Next.Method().Name);
            Assert.AreEqual(null, name);

            name = Get.ValueOrDefault(this, x => this.Fake.Method().Next.Method().Name);
            Assert.AreEqual(null, name);
        }

        [Test]
        public void GetNameWhenNotNull()
        {
            this.Fake = new Fake { Next = new Level { Name = "Johan" } };
            var name = Get.ValueOrDefault(() => this.Fake.Next.Name);
            Assert.AreEqual("Johan", name);

            name = Get.ValueOrDefault(this, x => this.Fake.Next.Name);
            Assert.AreEqual("Johan", name);
        }

        [Test]
        public void GetNameWhenNullExplicitDefaultValue()
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
            Assert.AreEqual(0, value);
        }

        [Test]
        public void GetValueWhenNullExplicit()
        {
            var value = Get.ValueOrDefault(() => this.Fake.Next.Value, int.MinValue);
            Assert.AreEqual(int.MinValue, value);

            value = Get.ValueOrDefault(this, x => this.Fake.Next.Value, int.MinValue);
            Assert.AreEqual(int.MinValue, value);
        }
    }
}