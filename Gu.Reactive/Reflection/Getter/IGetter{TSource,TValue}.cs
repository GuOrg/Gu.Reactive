namespace Gu.Reactive
{
    interface IGetter<TSource, TValue>
    {
        /// <summary>
        /// Get the value of the property for <paramref name="source"/>
        /// </summary>
        TValue GetValue(TSource source);

        /// <summary>
        /// Get the value of the property for <paramref name="source"/>
        /// </summary>
        Maybe<TValue> GetMaybe(TSource source);
    }
}