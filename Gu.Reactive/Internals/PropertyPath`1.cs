namespace Gu.Reactive.Internals
{
    using System;
    using System.Linq;

    internal class PropertyPath<TValue> : IMaybe<TValue>
    {
        private Maybe<TValue> _maybe;

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
            _maybe = propertyPath.GetValue(source).As<TValue>();
        }

        public bool HasValue
        {
            get { return _maybe.HasValue; }
        }

        public TValue Value
        {
            get { return _maybe.Value; }
        }
    }
}
