namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Reflection;

    public class Getter<TSource, TProperty> : IGetter
    {
        private readonly Func<TSource, TProperty> getter;

        private Getter(PropertyInfo property)
        {
            if (property.GetMethod == null)
            {
                throw new ArgumentException($"Expected get method to not be null. Property: {property}");
            }

            this.getter = (Func<TSource, TProperty>)Delegate.CreateDelegate(typeof(Func<TSource, TProperty>), property.GetMethod);
        }

        object IGetter.GetValue(object source)
        {
            return this.getter((TSource)source);
        }

        public TProperty GetValue(TSource source)
        {
            return this.getter((TSource)source);
        }
    }
}