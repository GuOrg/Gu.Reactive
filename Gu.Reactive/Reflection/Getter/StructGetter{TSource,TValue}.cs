﻿namespace Gu.Reactive
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
            if (property is { GetMethod: { } getMethod })
            {
                this.getter = (GetterDelegate?)Delegate.CreateDelegate(typeof(GetterDelegate), getMethod, throwOnBindFailure: true) ?? throw new InvalidOperationException("Failed creating delegate.");

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
