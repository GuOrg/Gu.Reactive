namespace Gu.Reactive
{
    using System;
    using System.Reflection;

    internal class StructGetter<TSource, TValue> : Getter<TSource, TValue>
        where TSource : struct
    {
        private readonly GetterDelegate getter;

        private StructGetter(PropertyInfo property)
            : base(property)
        {
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