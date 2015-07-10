namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;
    using Gu.Reactive.Internals;

    /// <summary>
    /// Base class for collections
    /// </summary>
    public abstract class ConditionCollection : IEnumerable<ICondition>, INotifyPropertyChanged, IDisposable
    {
        private readonly IReadOnlyList<ICondition> _innerConditions;
        private readonly IDisposable _subscription;
        private readonly Func<IReadOnlyList<ICondition>, bool?> _isSatisfied;
        private bool? _previousIsSatisfied;
        private bool _disposed;

        protected ConditionCollection(Func<IReadOnlyList<ICondition>, bool?> isSatisfied, params ICondition[] conditions)
        {
            Ensure.NotNull(isSatisfied, "isSatisfied");
            Ensure.NotNullOrEmpty(conditions, "conditions");
            
            if (conditions.Distinct().Count() != conditions.Length)
            {
                throw new ArgumentException("conditions must be distinct");
            }

            _isSatisfied = isSatisfied;
            _innerConditions = conditions.ToArray();
            _subscription = conditions.Select(x => x.ObservePropertyChanged(y => y.IsSatisfied, false))
                                       .Merge()
                                       .Subscribe(
                                           x =>
                                           {
                                               IsSatisfied = isSatisfied(_innerConditions);
                                           });
            _previousIsSatisfied = isSatisfied(_innerConditions);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsSatisfied
        {
            get
            {
                VerifyDisposed();
                return _isSatisfied(_innerConditions); // No caching
            }

            private set
            {
                // This is only to raise inpc, value is always calculated
                if (_previousIsSatisfied == value)
                {
                    return;
                }

                _previousIsSatisfied = value;
                OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override string ToString()
        {
            return string.Format("IsSatisfied: {0} {{{1}}}", IsSatisfied, string.Join(", ", _innerConditions.Select(x => x.Name)));
        }

        public IEnumerator<ICondition> GetEnumerator()
        {
            VerifyDisposed();
            return _innerConditions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            VerifyDisposed();
            return GetEnumerator();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _subscription.Dispose();
            }

            _disposed = true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}