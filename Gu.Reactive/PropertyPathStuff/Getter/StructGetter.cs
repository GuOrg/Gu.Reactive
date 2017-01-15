namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Reflection;

    internal class StructGetter<TSource, TValue> : Getter<TSource, TValue>
    {
        private readonly GetterDelegate getter;

        private StructGetter(PropertyInfo property)
            : base(property)
        {
            if (property.GetMethod == null)
            {
                throw new ArgumentException($"Expected get method to not be null. Property: {property}");
            }

            this.getter = (GetterDelegate)Delegate.CreateDelegate(typeof(GetterDelegate), property.GetMethod, true);
        }

        private delegate TValue GetterDelegate(ref TSource source);

        /// <inheritdoc/>
        public override TValue GetValue(TSource source)
        {
            return this.getter(ref source);
        }
    }
}