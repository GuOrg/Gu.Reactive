namespace Gu.Reactive
{
    /// <summary>
    ///
    /// </summary>
    public interface IConditionsService
    {
        /// <summary>
        /// Useful for returning mocks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ICondition Get<T>() where T : ICondition;
    }
}
