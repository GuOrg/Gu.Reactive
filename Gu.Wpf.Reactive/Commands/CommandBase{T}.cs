﻿namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// A base class for commands.
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public abstract class CommandBase<T> : ICommand, INotifyPropertyChanged
    {
        private bool isExecuting;

        /// <inheritdoc/>
        public virtual event EventHandler CanExecuteChanged
        {
            add => InternalCanExecuteChangedEventManager.AddHandler(this, value);

            remove => InternalCanExecuteChangedEventManager.RemoveHandler(this, value);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        private event EventHandler? InternalCanExecuteChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the command is currently executing.
        /// </summary>
        public bool IsExecuting
        {
            get => this.isExecuting;

            protected set
            {
                if (value == this.isExecuting)
                {
                    return;
                }

                this.isExecuting = value;
                this.OnPropertyChanged();
            }
        }

#pragma warning disable CA1033 // Interface methods should be callable by child types

        /// <inheritdoc/>
        bool ICommand.CanExecute(object? parameter) => this.InternalCanExecute((T?)parameter);

        /// <inheritdoc/>
        void ICommand.Execute(object? parameter) => this.InternalExecute((T?)parameter);
#pragma warning restore CA1033 // Interface methods should be callable by child types

        /// <summary>
        /// Raises the event on the Dispatcher if present. Safe to call from any thread.
        /// </summary>
#pragma warning disable CA1030 // Use events where appropriate
        public virtual void RaiseCanExecuteChanged()
#pragma warning restore CA1030 // Use events where appropriate
        {
            var handler = this.InternalCanExecuteChanged;
            if (handler != null)
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher != null)
                {
#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
                    _ = dispatcher.BeginInvoke(handler, this, EventArgs.Empty);
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
                }
                else
                {
                    handler.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Evaluates the criteria to see if the command can execute.
        /// </summary>
        /// <param name="parameter">The command parameter is passed as argument to the Criteria invocation.</param>
        /// <returns>A value indicating if the command can execute.</returns>
        protected abstract bool InternalCanExecute(T? parameter);

        /// <summary>
        /// Note to inheritors:
        /// This method must signal IsExecuting when starting/stopping
        /// IsExecuting = true;
        /// try
        /// {
        ///     action(...);
        /// }
        /// finally
        /// {
        ///     IsExecuting = false;
        /// }.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        protected abstract void InternalExecute(T? parameter);

        /// <summary>
        /// Raise the <see cref="PropertyChanged"/> event for <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to notify.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class InternalCanExecuteChangedEventManager : WeakEventManager
        {
            private static readonly InternalCanExecuteChangedEventManager Manager = new InternalCanExecuteChangedEventManager();

            static InternalCanExecuteChangedEventManager()
            {
                SetCurrentManager(typeof(InternalCanExecuteChangedEventManager), Manager);
            }

            internal static void AddHandler(CommandBase<T> source, EventHandler handler)
            {
                Manager.ProtectedAddHandler(source, handler);
            }

            internal static void RemoveHandler(CommandBase<T> source, EventHandler handler)
            {
                Manager.ProtectedRemoveHandler(source, handler);
            }

            protected override void StartListening(object source)
            {
                ((CommandBase<T>)source).InternalCanExecuteChanged += this.DeliverEvent;
            }

            protected override void StopListening(object source)
            {
                ((CommandBase<T>)source).InternalCanExecuteChanged -= this.DeliverEvent;
            }
        }
    }
}
