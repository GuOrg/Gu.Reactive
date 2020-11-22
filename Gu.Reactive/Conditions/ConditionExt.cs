namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    /// <summary>
    /// Extension methods for <see cref="ICondition"/>.
    /// </summary>
    public static class ConditionExt
    {
        /// <summary>
        /// Get an observable that notifies when ICondition.IsSatisfied changes.
        /// </summary>
        /// <param name="condition">The condition to track.</param>
        /// <returns>An observable that returns <paramref name="condition"/> every time ICondition.IsSatisfied changes.</returns>
        public static IObservable<T> ObserveIsSatisfiedChanged<T>(this T condition)
            where T : class, ISatisfied
        {
            return Observable.Create<T>(
                o =>
                {
                    void Handler(object _, PropertyChangedEventArgs e)
                    {
                        if (e.PropertyName == nameof(condition.IsSatisfied))
                        {
                            o.OnNext(condition);
                        }
                    }

                    condition.PropertyChanged += Handler;
                    return Disposable.Create(() => condition.PropertyChanged -= Handler);
                });
        }

        /// <summary>
        /// Return a condition that returns IsSatisfied false if <paramref name="condition"/>.IsSatisfied returns null.
        /// </summary>
        /// <typeparam name="T">The condition type.</typeparam>
        /// <param name="condition">The source condition.</param>
        /// <returns>A new instance of <see cref="NullIsFalse{T}"/>.</returns>
        public static NullIsFalse<T> NullIsFalse<T>(this T condition)
            where T : class, ICondition
        {
            return new NullIsFalse<T>(condition);
        }

        /// <summary>
        /// Returns true if history matches current state.
        /// </summary>
        /// <param name="condition">The condition.</param>
        public static bool IsInSync(this ICondition condition)
        {
            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            return condition.IsSatisfied == condition.History
                                                     .Last()
                                                     .State;
        }
    }
}
