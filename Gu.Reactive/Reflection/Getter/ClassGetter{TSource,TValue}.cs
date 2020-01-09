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
            if (property is { GetMethod: { } getMethod })
            {
                this.getter = (Func<TSource, TValue>?)Delegate.CreateDelegate(typeof(Func<TSource, TValue>), getMethod, throwOnBindFailure: true) ?? throw new InvalidOperationException($"Could not create getter delegate for {property}.");
            }
            else
            {
                throw new InvalidOperationException("Property does not have a get method.");
            }
        }

        /// <inheritdoc/>
        public override TValue GetValue(TSource source)
        {
            return this.getter(source);
        }
    }
}
