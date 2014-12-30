// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionCollection.cs" company="">
//   
// </copyright>
// <summary>
//   Base class for collections
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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

    /// <summary>
    /// Base class for collections
    /// </summary>
    public abstract class ConditionCollection : IEnumerable<ICondition>, INotifyPropertyChanged, IDisposable
    {
        private readonly List<ICondition> _innerConditions;
        private readonly IDisposable _subscription;
        private readonly Func<IEnumerable<ICondition>, bool?> _isSatisfied;
        private bool? _previousIsSatisfied;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionCollection"/> class.
        /// </summary>
        /// <param name="isSatisfied">To avoid virtual call</param>
        /// <param name="conditions">
        /// The conditions.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        protected ConditionCollection(Func<IEnumerable<ICondition>, bool?> isSatisfied, params ICondition[] conditions)
        {
            if (conditions == null)
            {
                throw new ArgumentNullException("conditions");
            }

            if (!conditions.Any())
            {
                throw new ArgumentException("Conditions cannot be empty", "conditions");
            }

            if (conditions.Distinct().Count() != conditions.Length)
            {
                throw new ArgumentException("conditions must be distinct");
            }

            if (isSatisfied == null)
            {
                throw new ArgumentNullException("isSatisfied");
            }

            _isSatisfied = isSatisfied;
            _innerConditions = conditions.ToList();
            _subscription = conditions.Select(x => x.ToObservable(y => y.IsSatisfied, false))
                                       .Merge()
                                       .Subscribe(
                                           x =>
                                           {
                                               IsSatisfied = isSatisfied(_innerConditions);
                                           });
            _previousIsSatisfied = isSatisfied(_innerConditions);
        }

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the is satisfied.
        /// </summary>
        public bool? IsSatisfied
        {
            get
            {
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

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("IsSatisfied: {0} {{{1}}}", IsSatisfied, string.Join(", ", _innerConditions.Select(x => x.Name)));
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        public IEnumerator<ICondition> GetEnumerator()
        {
            return _innerConditions.GetEnumerator();
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                foreach (var innerCondition in _innerConditions)
                {
                    innerCondition.Dispose();
                }

                _innerConditions.Clear();
                _subscription.Dispose();
            }

            _disposed = true;
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}