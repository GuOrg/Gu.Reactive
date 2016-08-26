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
                throw new InvalidOperationException(
                    string.Format(
                        "Valuepath type does not match. Expected: {0} was: {1}",
                        typeof(TValue).FullName,
                        last.PropertyInfo.PropertyType.FullName));
            }

            this.maybe = propertyPath.GetValue<TValue>(source);
        }

        public bool HasValue => this.maybe.HasValue;

        public TValue Value => this.maybe.Value;
    }
}
