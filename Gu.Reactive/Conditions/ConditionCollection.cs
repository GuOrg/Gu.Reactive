// ReSharper disable MemberCanBePrivate.Global
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
    /// Base class for collections of conditions
    /// </summary>
    public abstract class ConditionCollection : ISatisfied, IReadOnlyList<ICondition>, INotifyPropertyChanged, IDisposable
    {
        private readonly IReadOnlyList<ICondition> prerequisites;
        private readonly IDisposable subscription;
        private readonly Func<IReadOnlyList<ICondition>, bool?> isSatisfied;
        private bool? previousIsSatisfied;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionCollection"/> class.
        /// </summary>
        protected ConditionCollection(Func<IReadOnlyList<ICondition>, bool?> isSatisfied, params ICondition[] prerequisites)
        {
            Ensure.NotNull(isSatisfied, nameof(isSatisfied));
            Ensure.NotNullOrEmpty(prerequisites, nameof(prerequisites));

            if (prerequisites.Distinct().Count() != prerequisites.Length)
            {
                throw new ArgumentException("Prerequisites must be distinct", nameof(prerequisites));
            }

            this.isSatisfied = isSatisfied;
            this.prerequisites = prerequisites;
            this.subscription = prerequisites.Select(x => x.ObserveIsSatisfiedChanged())
                                       .Merge()
                                       .Subscribe(_ => this.IsSatisfied = isSatisfied(this.prerequisites));
            this.previousIsSatisfied = isSatisfied(this.prerequisites);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public int Count => this.prerequisites.Count;

        /// <inheritdoc/>
        public bool? IsSatisfied
        {
            get
            {
                this.ThrowIfDisposed();
                return this.isSatisfied(this.prerequisites); // No caching
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
        public ICondition this[int index] => this.prerequisites[index];

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public override string ToString() => $"IsSatisfied: {this.IsSatisfied} {{{string.Join(", ", this.prerequisites.Select(x => x.Name))}}}";

        /// <inheritdoc/>
        public IEnumerator<ICondition> GetEnumerator()
        {
            this.ThrowIfDisposed();
            return this.prerequisites.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            this.ThrowIfDisposed();
            return this.GetEnumerator();
        }

        internal static ICondition[] Prepend(ICondition condition, ICondition[] conditions)
        {
            Ensure.NotNull(condition, nameof(condition));
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