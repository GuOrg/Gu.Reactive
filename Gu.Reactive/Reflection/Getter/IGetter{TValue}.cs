namespace Gu.Reactive
{
    using System.Reflection;

    /// <summary>
    /// A wrapper around a delegate created from a <see cref="System.Reflection.PropertyInfo.GetMethod"/>.
    /// </summary>
    public interface IGetter<TValue>
    {
        /// <summary>
        /// The <see cref="PropertyInfo"/> that this instance is a getter for.
        /// </summary>
        PropertyInfo Property { get; }

        /// <summary>
        /// Get the value of the property for <paramref name="source"/>.
        /// </summary>
        Maybe<TValue> GetMaybe(object source);
    }
}