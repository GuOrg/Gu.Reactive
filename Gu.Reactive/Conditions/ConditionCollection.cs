// ReSharper disable MemberCanBePrivate.Global
namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Text;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Base class for collections of conditions
    /// </summary>
    public abstract class ConditionCollection : ReadOnlySerialViewBase<ICondition>, ISatisfied
    {
        private readonly Func<IReadOnlyList<ICondition>, bool?> isSatisfied;
        private readonly IDisposable subscription;

        private bool? previousIsSatisfied;

#pragma warning disable GU0003 // Name the parameters to match the members.
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionCollection"/> class.
        /// </summary>
        /// <param name="isSatisfied">The predicate for calculating if the collection is satisfied.</param>
        /// <param name="prerequisites">The children.</param>
        /// <param name="leaveOpen">True to not dispose <paramref name="prerequisites"/> when this instance is disposed.</param>
        protected ConditionCollection(Func<IReadOnlyList<ICondition>, bool?> isSatisfied, IReadOnlyList<ICondition> prerequisites, bool leaveOpen)
#pragma warning restore GU0003 // Name the parameters to match the members.
            : base(prerequisites, TimeSpan.Zero, ImmediateScheduler.Instance, leaveOpen)
        {
            Ensure.NotNull(isSatisfied, nameof(isSatisfied));
            Ensure.NotNull(prerequisites, nameof(prerequisites));

            var set = IdentitySet.Borrow<ICondition>();
            set.UnionWithWithRetries(prerequisites);
            if (set.Count != prerequisites.Count)
            {
                var builder = new StringBuilder();
                builder.AppendLine("Prerequisites must be distinct");
                foreach (var prerequisite in set)
                {
                    var count = prerequisites.Count(x => x == prerequisite);
                    if (count > 1)
                    {
                        builder.AppendLine($"{prerequisite.GetType().PrettyName()} appears {count} times");
                    }
                }

                throw new ArgumentException(builder.ToString(), nameof(prerequisites));
            }

            IdentitySet.Return(set);

            this.isSatisfied = isSatisfied;
            this.subscription = this.ObserveItemPropertyChangedSlim(x => x.IsSatisfied)
                                    .Subscribe(_ => this.IsSatisfied = this.isSatisfied(this));
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