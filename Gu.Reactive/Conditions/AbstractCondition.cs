﻿// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reactive.Disposables;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Internals;

    /// <summary>
    /// This is a baseclass when you want to have a nonstatic Criteria method.
    /// </summary>
    public abstract class AbstractCondition : ICondition
    {
        private readonly Lazy<Condition> condition;
        private readonly SingleAssignmentDisposable subscription = new SingleAssignmentDisposable();

        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCondition"/> class.
        /// </summary>
        /// <param name="observable">The <see cref="IObservable{Object}"/>.</param>
        protected AbstractCondition(IObservable<object> observable)
        {
            this.condition = new Lazy<Condition>(
                () =>
                    {
                        var created = new Condition(observable, this.Criteria);
                        this.subscription.Disposable = created.ObserveIsSatisfiedChanged()
                                                              .Subscribe(_ => this.OnPropertyChanged(nameof(this.IsSatisfied)));
                        return created;
                    });

            this.name = this.GetType().Name;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                this.condition.ForceCreate();
                this.PropertyChangedCore += value;
            }

            remove => this.PropertyChangedCore -= value;
        }

        private event PropertyChangedEventHandler? PropertyChangedCore;

        /// <inheritdoc/>
        public bool? IsSatisfied => this.condition.Value.IsSatisfied;

#pragma warning disable CA1033 // Interface methods should be callable by child types
        /// <inheritdoc/>
        IReadOnlyList<ICondition> ICondition.Prerequisites => this.condition.Value.Prerequisites;
#pragma warning restore CA1033 // Interface methods should be callable by child types

        /// <inheritdoc/>
        public IEnumerable<ConditionHistoryPoint> History
        {
            get
            {
                this.ThrowIfDisposed();
                return this.condition.Value.History;
            }
        }

        /// <inheritdoc/>
        public string Name
        {
            get
            {
                this.ThrowIfDisposed();
                return this.name;
            }

            set
            {
                this.ThrowIfDisposed();
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <inheritdoc/>
        public ICondition Negate()
        {
            this.ThrowIfDisposed();
            return new NegatedCondition(this);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Calls ForceCreate on the condition.
        /// This can be useful in the end of the constructor of inheriting class to start collecting history.
        /// </summary>
        protected void Initialize()
        {
            this.condition.ForceCreate();
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.IsDisposed = true;
            if (disposing)
            {
                if (this.condition.IsValueCreated)
                {
                    this.condition.Value.Dispose();
                    this.subscription?.Dispose();
                }
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
        /// The criteria for <see cref="IsSatisfied"/>.
        /// </summary>
        /// <returns>If the condition is satisfied.</returns>
        protected abstract bool? Criteria();

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChangedCore?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
