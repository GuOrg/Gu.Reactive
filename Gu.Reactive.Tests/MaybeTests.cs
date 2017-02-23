namespace Gu.Reactive.Tests
{
    using NUnit.Framework;

    public class MaybeTests
    {
        [Test]
        public void EqualityWhenNone()
        {
            Assert.AreEqual(Maybe<int>.None, Maybe<int>.None);
            Assert.AreEqual(true, Maybe<int>.None == Maybe<int>.None);
            Assert.AreEqual(false, Maybe<int>.None != Maybe<int>.None);
            Assert.AreEqual(0, Maybe<int>.None.GetHashCode());
        }

        [Test]
        public void EqualityWhenSome()
        {
            Assert.AreEqual(Maybe.Some(1), Maybe.Some(1));
            Assert.AreNotEqual(Maybe.Some(1), Maybe.Some(2));
            Assert.AreEqual(true, Maybe.Some(1) == Maybe.Some(1));
            Assert.AreEqual(false, Maybe.Some(1) == Maybe.Some(2));
            Assert.AreEqual(false, Maybe.Some(1) != Maybe.Some(1));
            Assert.AreEqual(true, Maybe.Some(1) != Maybe.Some(2));
            Assert.AreEqual(1, Maybe.Some(1).GetHashCode());
        }
    }
}