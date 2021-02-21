namespace Gu.Reactive
{
    using System;
    using System.Reflection;

    internal class StructGetter<TSource, TValue> : Getter<TSource, TValue>
        where TSource : struct
    {
        private readonly GetterDelegate getter;

#pragma warning disable IDE0051 // Remove unused private members
        private StructGetter(PropertyInfo property)
#pragma warning restore IDE0051 // Remove unused private members
            : base(property)
        {
            if (property is { GetMethod: { } getMethod })
            {
#pragma warning disable CS8600, CA1508 // Avoid dead conditional code
                this.getter = (GetterDelegate)Delegate.CreateDelegate(typeof(GetterDelegate), getMethod, throwOnBindFailure: true) ?? throw new InvalidOperationException("Failed creating delegate.");
#pragma warning restore CS8600, CA1508 // Avoid dead conditional code
            }
            else
            {
                throw new InvalidOperationException("Property does not have a get method.");
            }
        }

        private delegate TValue GetterDelegate(ref TSource source);

        /// <inheritdoc/>
        public override TValue GetValue(TSource source)
        {
            return this.getter(ref source);
        }
    }
}
