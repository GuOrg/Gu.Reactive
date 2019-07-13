// ReSharper disable MemberCanBePrivate.Global
namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// A negated condition wraps a <see cref="ICondition"/> and negates <see cref="IsSatisfied"/>.
    /// Calling Negate on it returns the original condition.
    /// </summary>
    public class NegatedCondition : ICondition
    {
        private readonly FixedSizedQueue<ConditionHistoryPoint> history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private readonly ICondition condition;
        private readonly IDisposable subscription;
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="NegatedCondition"/> class.
        /// </summary>
        public NegatedCondition(ICondition condition)
        {
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
            this.name = $"Not_{condition.Name}";

            this.subscription = condition.ObserveIsSatisfiedChanged()
                                         .Subscribe(
                                             _ =>
                                                 {
                                                     this.OnPropertyChanged(nameof(this.IsSatisfied));
                                                     this.history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, this.IsSatisfied));
                                                 });
            this.history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, this.IsSatisfied));
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public bool? IsSatisfied
        {
            get
            {
                this.ThrowIfDisposed();
                var isSatisfied = this.condition.IsSatisfied;
                if (isSatisfied == null)
                {
                    return null;
                }

                return isSatisfied != true;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<ICondition> Prerequisites => this.condition.Prerequisites;

        /// <inheritdoc/>
        public IEnumerable<ConditionHistoryPoint> History => this.history;

        /// <inheritdoc/>
        public string Name
        {
            get => this.name;

            set
            {
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// True if this instance is disposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Returns the negated (original) condition.
        /// </summary>
        [Obsolete("This will be made explicit.")]
        public ICondition Negate()
        {
            this.ThrowIfDisposed();
            return this.condition;
        }

        /// <inheritdoc/>
        public override string ToString() => $"Name: {(string.IsNullOrEmpty(this.Name) ? this.GetType().PrettyName() : this.Name)}, IsSatisfied: {this.IsSatisfied?.ToString(CultureInfo.InvariantCulture) ?? "null"}";

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of a <see cref="NegatedCondition"/>.
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
            if (this.IsDisposed)
            {
                return;
            }

            this.IsDisposed = true;
            if (disposing)
            {
                this.subscription.Dispose();
            }
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// </summary>
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
