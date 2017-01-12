namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// A negated condition wraps a <see cref="ICondition"/> and negates <see cref="IsSatisfied"/>.
    /// Calling Negate on it returns the original condition.
    /// </summary>
    public sealed class NegatedCondition : ICondition
    {
        private readonly FixedSizedQueue<ConditionHistoryPoint> history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private readonly ICondition condition;
        private readonly IDisposable subscription;
        private string name;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="NegatedCondition"/> class.
        /// </summary>
        public NegatedCondition(ICondition condition)
        {
            this.condition = condition;
            this.Name = $"Not_{this.condition.Name}";

            this.subscription = condition.ObserveIsSatisfiedChanged()
                                         .Subscribe(_ => this.OnPropertyChanged(nameof(this.IsSatisfied)));

            this.ObservePropertyChangedSlim(nameof(this.IsSatisfied), true)
                .Subscribe(_ => this.history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, this.IsSatisfied)));
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public bool? IsSatisfied
        {
            get
            {
                var isSatisfied = this.condition.IsSatisfied;
                if (isSatisfied == null)
                {
                    return null;
                }

                return isSatisfied != true;
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                return this.name;
            }

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

        /// <inheritdoc/>
        public IReadOnlyList<ICondition> Prerequisites => this.condition.Prerequisites;

        /// <inheritdoc/>
        public IEnumerable<ConditionHistoryPoint> History => this.history;

        /// <summary>
        /// Returns the negated (original) condition.
        /// </summary>
        public ICondition Negate() => this.condition;

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.subscription.Dispose();
        }

        /// <inheritdoc/>
        public override string ToString() => $"Name: {(string.IsNullOrEmpty(this.Name) ? this.GetType().PrettyName() : this.Name)}, IsSatisfied: {this.IsSatisfied?.ToString() ?? "null"}";

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
