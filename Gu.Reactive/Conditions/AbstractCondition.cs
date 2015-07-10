namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    /// <summary>
    /// This is a baseclass when you want to have a nonstatic Criteria method
    /// </summary>
    public abstract class AbstractCondition : ICondition
    {
        private readonly Condition _condition;
        private bool _disposed;

        protected AbstractCondition(IObservable<object> observable)
        {
            _condition = new Condition(observable, Criteria);
            _condition.ObservePropertyChanged()
                      .Subscribe(x => OnPropertyChanged(x.EventArgs));
            Name = _condition.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsSatisfied
        {
            get { return _condition.IsSatisfied; }
        }

        public string Name { get; protected set; }

        public IEnumerable<ICondition> Prerequisites
        {
            get
            {
                VerifyDisposed();
                return _condition.Prerequisites;
            }
        }

        public IEnumerable<ConditionHistoryPoint> History
        {
            get { return _condition.History; }
        }

        public ICondition Negate()
        {
            VerifyDisposed();
            return _condition.Negate();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (disposing)
            {
                _condition.Dispose();
            }
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        protected abstract bool? Criteria();

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Calls nameof internally
        /// </summary>
        /// <param name="propety"></param>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propety)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(NameOf.Property(propety)));
            }
        }
    }
}