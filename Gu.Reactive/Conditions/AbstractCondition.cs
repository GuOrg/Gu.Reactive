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
        private readonly Condition condition;
        private bool disposed;
        private string name;

        protected AbstractCondition(IObservable<object> observable)
        {
            this.condition = new Condition(observable, this.Criteria);
            this.condition.ObservePropertyChanged()
                      .Subscribe(x => this.OnPropertyChanged(x.EventArgs));
            this.Name = this.condition.Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsSatisfied => this.condition.IsSatisfied;

        public string Name
        {
            get
            {
                this.VerifyDisposed();
                return this.name;
            }

            set
            {
                this.VerifyDisposed();
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged();
            }
        }

        public IReadOnlyList<ICondition> Prerequisites
        {
            get
            {
                this.VerifyDisposed();
                return this.condition.Prerequisites;
            }
        }

        public IEnumerable<ConditionHistoryPoint> History => this.condition.History;

        public ICondition Negate()
        {
            this.VerifyDisposed();
            return this.condition.Negate();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.condition.Dispose();
            }
        }

        protected void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        protected abstract bool? Criteria();

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Calls nameof internally
        /// </summary>
        /// <param name="propety"></param>
        [Obsolete("Use nameof")]
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propety)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NameOf.Property(propety)));
        }
    }
}