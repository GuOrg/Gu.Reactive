namespace Gu.Reactive.Tests.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class PropertyInfoComparerTests
    {
        [TestCase(typeof(Fake), "Value")]
        public void Equals(Type type, string propertyName)
        {
            var propertyInfo1 = type.GetProperty(propertyName);
            var propertyInfo2 = type.GetProperty(propertyName);
            Assert.AreEqual(true, PropertyInfoComparer.Equals(propertyInfo1, propertyInfo2));
            Assert.AreEqual(PropertyInfoComparer.GetHashCode(propertyInfo1), PropertyInfoComparer.GetHashCode(propertyInfo2));

            var comparer = (IEqualityComparer<PropertyInfo>)PropertyInfoComparer.Default;
            Assert.AreEqual(true, comparer.Equals(propertyInfo1, propertyInfo2));
            Assert.AreEqual(comparer.GetHashCode(propertyInfo1), comparer.GetHashCode(propertyInfo2));
        }

        [TestCase(typeof(Fake), "Value", typeof(Fake), "Name")]
        [TestCase(typeof(Fake), "Value", typeof(Level), "Name")]
        [TestCase(typeof(Fake), "Value", typeof(Fake<double>), "Value")]
        [TestCase(typeof(Fake<int>), "Value", typeof(Fake<double>), "Value")]
        public void NotEquals(Type type1, string propertyName1, Type type2, string propertyName2)
        {
            var propertyInfo1 = type1.GetProperty(propertyName1);
            var propertyInfo2 = type2.GetProperty(propertyName2);
            Assert.AreEqual(false, PropertyInfoComparer.Equals(propertyInfo1, propertyInfo2));
            Assert.AreEqual(false, ((IEqualityComparer<PropertyInfo>)PropertyInfoComparer.Default).Equals(propertyInfo1, propertyInfo2));
        }
    }
}