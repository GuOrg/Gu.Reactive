namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// The negated condition. Calling Negate on it returns the original condition.
    /// </summary>
    public sealed class NegatedCondition : ICondition
    {
        private readonly FixedSizedQueue<ConditionHistoryPoint> history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private readonly ICondition condition;
        private readonly IDisposable subscription;
        private string name;

        private bool disposed;

        public NegatedCondition(Condition condition)
        {
            this.condition = condition;
            this.Name = $"Not_{this.condition.Name}";

            this.subscription = condition.ObserveIsSatisfiedChanged()
                                     .Subscribe(_ => this.OnPropertyChanged(nameof(this.IsSatisfied)));

            this.ObservePropertyChangedSlim(nameof(this.IsSatisfied), true)
                .Subscribe(_ => this.history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, this.IsSatisfied)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        /// <summary>
        /// Gets the prerequisites.
        /// </summary>
        public IReadOnlyList<ICondition> Prerequisites => this.condition.Prerequisites;

        /// <summary>
        /// Gets the history.
        /// </summary>
        public IEnumerable<ConditionHistoryPoint> History => this.history;

        public ICondition Negate() => this.condition;

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.subscription.Dispose();
            // GC.SuppressFinalize(this);
        }

        private void OnPropertyChanged(string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
