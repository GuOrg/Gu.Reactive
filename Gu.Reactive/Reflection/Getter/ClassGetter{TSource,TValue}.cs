namespace Gu.Reactive
{
    using System;
    using System.Reflection;

    internal class ClassGetter<TSource, TValue> : Getter<TSource, TValue>
        where TSource : class
    {
        private readonly Func<TSource, TValue> getter;

        protected ClassGetter(PropertyInfo property)
            : base(property)
        {
            this.getter = (Func<TSource, TValue>)Delegate.CreateDelegate(typeof(Func<TSource, TValue>), property.GetMethod, throwOnBindFailure: true);
        }

        /// <inheritdoc/>
        public override TValue GetValue(TSource source)
        {
            return this.getter(source);
        }
    }
}