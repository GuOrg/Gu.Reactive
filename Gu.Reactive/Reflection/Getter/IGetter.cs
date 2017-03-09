namespace Gu.Reactive
{
    using System.Reflection;

    /// <summary>
    /// A wrapper around a delegate created from a <see cref="System.Reflection.PropertyInfo.GetMethod"/>
    /// </summary>
    public interface IGetter
    {
        PropertyInfo Property { get; }

        /// <summary>
        /// Get the value of the property for <paramref name="source"/>
        /// </summary>
        object GetValue(object source);

        /// <summary>
        /// Get the value of the property for <paramref name="source"/>
        /// </summary>
        Maybe<object> GetMaybe(object source);
    }
}