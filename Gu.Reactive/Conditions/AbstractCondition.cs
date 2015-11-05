namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    /// <summary>
    /// This is a baseclass when you want to have a nonstatic Criteria method
    /// </summary>
    public abstract class AbstractCondition : ICondition
    {
        private readonly Condition _condition;
        private bool _disposed;
        private string _name;

        protected AbstractCondition(IObservable<object> observable)
        {
            _condition = new Condition(observable, Criteria);
            _condition.ObservePropertyChanged()
                      .Subscribe(x => OnPropertyChanged(x.EventArgs));
            Name = _condition.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsSatisfied => _condition.IsSatisfied;

        public string Name
        {
            get
            {
                VerifyDisposed();
                return _name;
            }

            set
            {
                VerifyDisposed();
                if (value == _name)
                {
                    return;
                }

                _name = value;
                OnPropertyChanged();
            }
        }

        public IReadOnlyList<ICondition> Prerequisites
        {
            get
            {
                VerifyDisposed();
                return _condition.Prerequisites;
            }
        }

        public IEnumerable<ConditionHistoryPoint> History => _condition.History;

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
            PropertyChanged?.Invoke(this, e);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Calls nameof internally
        /// </summary>
        /// <param name="propety"></param>
        [Obsolete("Use nameof")]
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propety)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NameOf.Property(propety)));
        }
    }
}