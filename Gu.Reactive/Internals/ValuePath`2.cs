namespace Gu.Reactive.Internals
{
    using System;
    using System.Linq;

    internal class ValuePath<TSource, TValue> : IValuePath<TSource, TValue>
    {
        private readonly ValuePath _valuePath;
        private readonly object _lock = new object();
        public ValuePath(ValuePath valuePath)
        {
            var last = (PathItem)valuePath.Last();
            if (last.PropertyInfo.PropertyType != typeof(TValue))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Valuepath type does not match. Expected: {0} was: {1}",
                        typeof(TValue).FullName,
                        last.PropertyInfo.PropertyType.FullName));
            }
            _valuePath = valuePath;
        }

        public IMaybe<TValue> Value(TSource source)
        {
            lock (_lock)
            {
                _valuePath.Source = source;
                var lastSource = _valuePath.LastSource as PathItem;
                if (lastSource == null)
                {
                    return new Maybe<TValue>(_valuePath.HasValue, (TValue)_valuePath.ValueOrDefault);
                }
                // This is an optimization. Profiler pointed here, minor issue.
                var value = lastSource.Value;
                return new Maybe<TValue>(value != null , (TValue) lastSource.Next.GetValue(value));
            }
        }
    }
}
