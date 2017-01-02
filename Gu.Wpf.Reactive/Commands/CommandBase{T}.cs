namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Concurrency;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using JetBrains.Annotations;

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
            add
            {
                InternalCanExecuteChangedEventManager.AddHandler(this, value);
            }

            remove
            {
                InternalCanExecuteChangedEventManager.RemoveHandler(this, value);
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private event EventHandler InternalCanExecuteChanged;

        /// <summary>
        /// Gets a value indicating if the command is currently executing.
        /// </summary>
        public bool IsExecuting
        {
            get
            {
                return this.isExecuting;
            }

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

        /// <inheritdoc/>
        bool ICommand.CanExecute(object parameter)
        {
            return this.InternalCanExecute((T)parameter);
        }

        /// <inheritdoc/>
        void ICommand.Execute(object parameter)
        {
            this.InternalExecute((T)parameter);
        }

        /// <summary>
        /// Raises the event on the Dispatcher if present. Safe to call from any thread.
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            var handler = this.InternalCanExecuteChanged;
            if (handler != null)
            {
                var scheduler = Schedulers.DispatcherOrCurrentThread;
                scheduler.Schedule(() => handler(this, EventArgs.Empty));
            }
        }

        protected abstract bool InternalCanExecute(T parameter);

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
        /// }
        /// </summary>
        /// <param name="parameter">The command parameter</param>
        protected abstract void InternalExecute(T parameter);

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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

            ////protected override ListenerList NewListenerList()
            ////{
            ////    return new ListenerList();
            ////}
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