namespace Gu.Reactive
{
    /// <summary>
    /// A property path.
    /// </summary>
    public interface IValuePath<in TSource, out TValue>
    {
        /// <summary>
        /// Get the value recursively from the root.
        /// </summary>
        IMaybe<TValue> GetValue(TSource source);
    }
}