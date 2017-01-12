namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Linq;
    using Internals;

    internal class PropertyPath<TValue> : IMaybe<TValue>
    {
        private readonly Maybe<TValue> maybe;

        internal PropertyPath(PropertyPath propertyPath, object source)
        {
            var last = propertyPath.Last();
            if (last.PropertyInfo.PropertyType != typeof(TValue))
            {
                var message = $"Valuepath type does not match. Expected: {typeof(TValue).FullName} was: {last.PropertyInfo.PropertyType.FullName}";
                throw new InvalidOperationException(message);
            }

            this.maybe = propertyPath.GetValue<TValue>(source);
        }

        public bool HasValue => this.maybe.HasValue;

        public TValue Value => this.maybe.Value;
    }
}
