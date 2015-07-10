namespace Gu.Reactive.Tests.Internals
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    public class TypeExtTests
    {
        [TestCase(typeof(double), "double")]
        [TestCase(typeof(Nullable<double>), "Nullable<double>")]
        [TestCase(typeof(IDictionary<string, double>), "IDictionary<string, double>")]
        public void PrettyName(Type type, string expected)
        {
            var actual = type.PrettyName();
            Assert.AreEqual(expected, actual);
        }
    }
}
