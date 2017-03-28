// ReSharper disable MemberCanBePrivate.Global
namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Base class for collections of conditions
    /// </summary>
    public abstract class ConditionCollection : ReadonlyViewBase<ICondition, ICondition>, ISatisfied
    {
        private readonly IDisposable subscription;
        private readonly Func<IReadOnlyList<ICondition>, bool?> isSatisfied;
        private bool? previousIsSatisfied;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionCollection"/> class.
        /// </summary>
        /// <param name="isSatisfied">The predicate for calculating if the collection is satisfied.</param>
        /// <param name="prerequisites">The children.</param>
        /// <param name="leaveOpen">True to not dispose <paramref name="prerequisites"/> when this instance is disposed.</param>
        protected ConditionCollection(Func<IReadOnlyList<ICondition>, bool?> isSatisfied, IReadOnlyList<ICondition> prerequisites, bool leaveOpen)
            : base(prerequisites, s => s, leaveOpen, false)
        {
            Ensure.NotNull(isSatisfied, nameof(isSatisfied));
            Ensure.NotNull(prerequisites, nameof(prerequisites));

            if (prerequisites.Distinct().Count() != prerequisites.Count)
            {
                throw new ArgumentException("Prerequisites must be distinct", nameof(prerequisites));
            }

            this.isSatisfied = isSatisfied;
            this.subscription = this.ObserveCollectionChangedSlim(true)
                                    .Select(_ => this.Select(c => c.ObserveIsSatisfiedChanged())
                                                     .Merge())
                                    .Switch()
                                    .Skip(1)
                                    .Subscribe(_ => this.IsSatisfied = isSatisfied(this));
            this.previousIsSatisfied = isSatisfied(this);
        }

        /// <inheritdoc/>
        public bool? IsSatisfied
        {
            get
            {
                this.ThrowIfDisposed();
                return this.isSatisfied(this); // No caching
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
        public override string ToString() => $"IsSatisfied: {this.IsSatisfied} {{{string.Join(", ", this.Select(x => x.Name))}}}";

        internal static IReadOnlyList<ICondition> Prepend(ICondition condition1, ICondition condition2, ICondition[] conditions)
        {
            Ensure.NotNull(condition1, nameof(condition1));
            Ensure.NotNull(condition2, nameof(condition2));
            if (conditions == null || conditions.Length == 0)
            {
                return new[] { condition1, condition2 };
            }

            var result = new ICondition[conditions.Length + 2];
            result[0] = condition1;
            result[1] = condition2;
            conditions.CopyTo(result, 2);
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
        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.subscription.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}