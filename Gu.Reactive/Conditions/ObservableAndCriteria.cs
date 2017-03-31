namespace Gu.Reactive
{
    using System;

    public struct ObservableAndCriteria
    {
        public ObservableAndCriteria(IObservable<object> observable, Func<bool?> criteria)
        {
            this.Observable = observable;
            this.Criteria = criteria;
        }

        public IObservable<object> Observable { get;  }

        public Func<bool?> Criteria { get;  }
    }
}