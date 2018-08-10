namespace Gu.Reactive
{
    /// <summary>
    /// A class that can be use to wrap an IoC container.
    /// </summary>
    public interface IConditionsService
    {
        /// <summary>
        /// Useful for returning mocks.
        /// </summary>
        /// <typeparam name="T">The type of condition to get.</typeparam>
        /// <returns>A condition of type <typeparamref name="T"/>.</returns>
        ICondition Get<T>()
            where T : ICondition;
    }
}
