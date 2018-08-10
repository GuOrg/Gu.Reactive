namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// An observable and a criteria for creating a <see cref="Condition"/>.
    /// </summary>
    public struct ObservableAndCriteria
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableAndCriteria"/> struct.
        /// </summary>
        public ObservableAndCriteria(IObservable<object> observable, Func<bool?> criteria)
        {
            this.Observable = observable;
            this.Criteria = criteria;
        }

        /// <summary>
        /// The observable that signals when criteria should be evaluated.
        /// </summary>
        public IObservable<object> Observable { get;  }

        /// <summary>
        /// The criteria evaluated by the condition to determine if satisfied.
        /// </summary>
        public Func<bool?> Criteria { get;  }
    }
}