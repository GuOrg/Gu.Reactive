﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NegatedCondition.cs" company="">
//   
// </copyright>
// <summary>
//   The negated condition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// The negated condition.
    /// </summary>
    public sealed class NegatedCondition : ICondition
    {
        private readonly FixedSizedQueue<ConditionHistoryPoint> _history = new FixedSizedQueue<ConditionHistoryPoint>(100);
        private readonly ICondition _condition;
        private readonly ICondition _innerNegated;
        private readonly IDisposable _subscription;
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="NegatedCondition"/> class.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        /// <param name="negated">
        /// The negated.
        /// </param>
        private NegatedCondition(ICondition condition, ICondition negated)
        {
            _condition = condition;
            _innerNegated = negated;
            _subscription = condition.ObservePropertyChanged(x => x.IsSatisfied, false)
                                          .Subscribe(
                                              x =>
                                                  {
                                                      _history.Enqueue(new ConditionHistoryPoint(DateTime.UtcNow, IsSatisfied));
                                                      OnPropertyChanged(x.EventArgs.PropertyName);
                                                  });
            Name = string.Format("Not_{0}", _condition.Name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NegatedCondition"/> class.
        /// </summary>
        /// <param name="condition">
        /// The condition.
        /// </param>
        public NegatedCondition(Condition condition)
            : this(
            (ICondition)condition, 
            new Condition(
                condition.ObservePropertyChanged(x => x.IsSatisfied), 
                () => condition.IsSatisfied == null ? (bool?)null : !condition.IsSatisfied.Value))
        {
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
                return _innerNegated.IsSatisfied;
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
        /// Gets the prerequisites.
        /// </summary>
        public IEnumerable<ICondition> Prerequisites
        {
            get
            {
                return _condition.Prerequisites;
            }
        }

        /// <summary>
        /// Gets the history.
        /// </summary>
        public IEnumerable<ConditionHistoryPoint> History
        {
            get
            {
                return _history;
            }
        }

        /// <summary>
        /// The negate.
        /// </summary>
        /// <returns>
        /// The <see cref="ICondition"/>.
        /// </returns>
        public ICondition Negate()
        {
            return _condition;
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
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _innerNegated.Dispose();
                _subscription.Dispose();
            }
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        private void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
