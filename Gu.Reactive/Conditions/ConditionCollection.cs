namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;

    /// <summary>
    /// Base class for collections
    /// </summary>
    public abstract class ConditionCollection : ISatisfied, IReadOnlyList<ICondition>, INotifyPropertyChanged, IDisposable
    {
        private readonly IReadOnlyList<ICondition> innerConditions;
        private readonly IDisposable subscription;
        private readonly Func<IReadOnlyList<ICondition>, bool?> isSatisfied;
        private bool? previousIsSatisfied;
        private bool disposed;

        protected ConditionCollection(Func<IReadOnlyList<ICondition>, bool?> isSatisfied, params ICondition[] conditions)
        {
            Ensure.NotNull(isSatisfied, nameof(isSatisfied));
            Ensure.NotNullOrEmpty(conditions, "conditions");

            if (conditions.Distinct().Count() != conditions.Length)
            {
                throw new ArgumentException("conditions must be distinct");
            }

            this.isSatisfied = isSatisfied;
            this.innerConditions = conditions;
            this.subscription = conditions.Select(x => x.ObserveIsSatisfiedChanged())
                                       .Merge()
                                       .Subscribe(
                                           x =>
                                           {
                                               this.IsSatisfied = isSatisfied(this.innerConditions);
                                           });
            this.previousIsSatisfied = isSatisfied(this.innerConditions);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public bool? IsSatisfied
        {
            get
            {
                this.ThrowIfDisposed();
                return this.isSatisfied(this.innerConditions); // No caching
            }

            private set
            {
                // This is only to raise inpc, value is always calculated
                if (this.previousIsSatisfied == value)
                {
                    return;
                }

                this.previousIsSatisfied = value;
                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public int Count => this.innerConditions.Count;

        /// <inheritdoc/>
        public ICondition this[int index] => this.innerConditions[index];

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public override string ToString() => $"IsSatisfied: {this.IsSatisfied} {{{string.Join(", ", this.innerConditions.Select(x => x.Name))}}}";

        /// <inheritdoc/>
        public IEnumerator<ICondition> GetEnumerator()
        {
            this.ThrowIfDisposed();
            return this.innerConditions.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            this.ThrowIfDisposed();
            return this.GetEnumerator();
        }

        internal static ICondition[] Prepend(ICondition condition, ICondition[] conditions)
        {
            Ensure.NotNullOrEmpty(conditions, nameof(conditions));
            var result = new ICondition[conditions.Length + 1];
            result[0] = condition;
            conditions.CopyTo(result, 1);
            return result;
        }

        /// <summary>
        /// Disposes of a <see cref="ConditionCollection"/>.
        /// </summary>
        /// <remarks>
        /// Called from Dispose() with disposing=true.
        /// Guidelines:
        /// 1. We may be called more than once: do nothing after the first call.
        /// 2. Avoid throwing exceptions if disposing is false, i.e. if we're being finalized.
        /// </remarks>
        /// <param name="disposing">True if called from Dispose(), false if called from the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.subscription.Dispose();
            }

            this.disposed = true;
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ConditionCollection"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
    }
}