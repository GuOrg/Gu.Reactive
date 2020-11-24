namespace Gu.Reactive
{
    using System.Reflection;

    /// <summary>
    /// A wrapper around a delegate created from a <see cref="PropertyInfo.GetMethod"/>.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TValue">The property type.</typeparam>
    public interface IGetter<in TSource, TValue>
    {
        /// <summary>
        /// Get the value of the property for <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source value.</param>
        /// <returns>The property value.</returns>
        TValue GetValue(TSource source);

        /// <summary>
        /// Get the value of the property for <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source value.</param>
        /// <returns>A <see cref="Maybe{TValue}"/>.</returns>
        Maybe<TValue> GetMaybe(TSource source);
    }
}
