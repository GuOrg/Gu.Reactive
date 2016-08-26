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
    using JetBrains.Annotations;

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

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsSatisfied
        {
            get
            {
                this.VerifyDisposed();
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

        public int Count => this.innerConditions.Count;

        public ICondition this[int index] => this.innerConditions[index];

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString() => $"IsSatisfied: {this.IsSatisfied} {{{string.Join(", ", this.innerConditions.Select(x => x.Name))}}}";

        public IEnumerator<ICondition> GetEnumerator()
        {
            this.VerifyDisposed();
            return this.innerConditions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.VerifyDisposed();
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.subscription.Dispose();
            }

            this.disposed = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}