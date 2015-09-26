namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// An async command that does not use command parameter
    /// </summary>
    public abstract class AsyncCommandBase<TTask> : ManualRelayCommand, IAsyncCommand<object>
        where TTask : Task
    {
        private readonly Func<TTask> _action;
        private readonly Func<TTask, NotifyTaskCompletionBase<TTask>> _creator;

        private readonly bool _disableMultipleRequests;
        private NotifyTaskCompletionBase<TTask> _execution;

        protected AsyncCommandBase(
            Func<TTask> action,
            Func<bool> condition,
            Func<TTask, NotifyTaskCompletionBase<TTask>> creator,
            bool disableMultipleRequests = true)
            : base(
                () => { throw new InvalidOperationException("Should not be called"); }, // Dummy action sent to base. Not super nice.
                condition)
        {
            _action = action;
            _creator = creator;
            _disableMultipleRequests = disableMultipleRequests;
        }

        protected AsyncCommandBase(
            Func<TTask> action,
            Func<TTask, NotifyTaskCompletionBase<TTask>> creator,
            bool disableMultipleRequests = true)
            : this(
                action,
                () => true,
                creator,
                disableMultipleRequests)
        {
        }

        public NotifyTaskCompletionBase<TTask> Execution
        {
            get { return _execution; }
            private set
            {
                if (Equals(_execution, value))
                {
                    return;
                }
                _execution = value;
                OnPropertyChanged();
            }
        }

        public async Task ExecuteAsync()
        {
            Execution = _creator(_action());
            RaiseCanExecuteChanged();
            await Execution.Completed;
        }

        Task IAsyncCommand<object>.ExecuteAsync(object parameter)
        {
            return ExecuteAsync();
        }

        protected override bool InternalCanExecute(object parameter)
        {
            if (_disableMultipleRequests)
            {
                if (Execution != null && !Execution.IsCompleted)
                {
                    return false;
                }
            }

            return base.InternalCanExecute(parameter);
        }

        protected override async void InternalExecute(object parameter)
        {
            await ExecuteAsync();
            RaiseCanExecuteChanged();
        }
    }
}