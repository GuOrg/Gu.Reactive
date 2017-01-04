namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

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
            this.condition.ObservePropertyChangedSlim()
                          .Subscribe(this.OnPropertyChanged);
            this.Name = this.condition.Name;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public bool? IsSatisfied => this.condition.IsSatisfied;

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                this.ThrowIfDisposed();
                return this.name;
            }

            set
            {
                this.ThrowIfDisposed();
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<ICondition> Prerequisites
        {
            get
            {
                this.ThrowIfDisposed();
                return this.condition.Prerequisites;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ConditionHistoryPoint> History => this.condition.History;

        /// <inheritdoc/>
        public ICondition Negate()
        {
            this.ThrowIfDisposed();
            return this.condition.Negate();
        }

        /// <inheritdoc/>
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

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// The criteria for <see cref="IsSatisfied"/>
        /// </summary>
        protected abstract bool? Criteria();

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="AbstractCondition"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}