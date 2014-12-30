﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Condition.cs" company="">
//   
// </copyright>
// <summary>
//   To be used standalone or derived from. Conditions really starts to sing when you subclass them and use an IoC container to build trees.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    /// <summary>
    /// To be used standalone or derived from. Conditions really starts to sing when you subclass them and use an IoC container to build trees.
    /// </summary>
    public class Condition : ICondition
    {
        private readonly Func<bool?> _criteria;
        private readonly IDisposable _subscription;
        private readonly FixedSizedQueue<ConditionHistoryPoint> _history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private bool? _isSatisfied;
        private string _name;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Condition"/> class.
        /// </summary>
        /// <param name="observable">
        /// The observable that triggers notifications
        /// </param>
        /// <param name="criteria">
        /// The criteria that is evaluated to give IsSatisfied.
        /// </param>
        public Condition(IObservable<object> observable, Func<bool?> criteria)
        {
            if (observable == null)
            {
                throw new ArgumentNullException("observable");
            }
            if (criteria == null)
            {
                throw new ArgumentNullException("criteria");
            }
            _criteria = criteria;
            Name = GetType().Name;
            _subscription = observable.Subscribe(x => UpdateIsSatisfied());
            UpdateIsSatisfied();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditionCollection"></param>
        protected Condition(ConditionCollection conditionCollection)
            : this(conditionCollection.ToObservable(x => x.IsSatisfied, false), () => conditionCollection.IsSatisfied)
        {
        }

        /// <summary>
        /// The property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Evaluates the criteria and returns if it is satisfied. 
        /// Notifies via PropertyChanged when it changes.
        /// </summary>
        public bool? IsSatisfied
        {
            get
            {
                return InternalIsSatisfied(); // No caching
            }

            private set
            {
                // This is only to raise inpc, value is always calculated
                if (_isSatisfied == value)
                {
                    return;
                }

                _isSatisfied = value;
                _history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, _isSatisfied));
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (value == _name)
                {
                    return;
                }

                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A log of the last 100 states and times
        /// </summary>
        public IEnumerable<ConditionHistoryPoint> History
        {
            get
            {
                return _history;
            }
        }

        /// <summary>
        /// The subconditions for this condition
        /// </summary>
        public IEnumerable<ICondition> Prerequisites
        {
            get
            {
                return Enumerable.Empty<ICondition>();
            }
        }

        /// <summary>
        /// Returns this condition negated, negating again returns the original
        /// </summary>
        /// <returns>
        /// The <see cref="ICondition"/>.
        /// </returns>
        public virtual ICondition Negate()
        {
            return new NegatedCondition(this);
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
            return string.Format("Name: {0}, IsSatisfied: {1}",
                string.IsNullOrEmpty(Name) ? GetType().Name : Name,
                IsSatisfied == null ? "null" : IsSatisfied.ToString());
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
                _subscription.Dispose();
            }
            _disposed = true;
        }

        /// <summary>
        /// The update is satisfied.
        /// </summary>
        protected void UpdateIsSatisfied()
        {
            IsSatisfied = InternalIsSatisfied();
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// The internal is satisfied.
        /// </summary>
        /// <returns>
        /// The <see cref="bool?"/>.
        /// </returns>
        private bool? InternalIsSatisfied()
        {
            return _criteria();
        }
    }
}
