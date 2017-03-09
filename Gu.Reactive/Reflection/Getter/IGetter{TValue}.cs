namespace Gu.Reactive
{
    /// <summary>
    /// A wrapper around a delegate created from a <see cref="System.Reflection.PropertyInfo.GetMethod"/>
    /// </summary>
    public interface IGetter<TValue> : IGetter
    {
        /// <summary>
        /// Get the value of the property for <paramref name="source"/>
        /// </summary>
        TValue GetValue(object source);

        /// <summary>
        /// Get the value of the property for <paramref name="source"/>
        /// </summary>
        Maybe<TValue> GetMaybe(object source);
    }
}