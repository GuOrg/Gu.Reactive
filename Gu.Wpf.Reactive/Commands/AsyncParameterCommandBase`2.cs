namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// An async command that uses a command parameter of type <typeparam name="TParameter"></typeparam>
    /// </summary>
    public abstract class AsyncParameterCommandBase<TParameter, TTask> : ManualRelayCommand<TParameter>, IAsyncCommand<TParameter>
        where TTask : Task
    {
        private readonly Func<TParameter, TTask> _action;

        private readonly Func<TTask, NotifyTaskCompletionBase<TTask>> _creator;

        private readonly bool _disableMultipleRequests;
        private NotifyTaskCompletionBase<TTask> _execution;

        protected AsyncParameterCommandBase(
            Func<TParameter, TTask> action,
            Func<TParameter,bool> condition,
            Func<TTask, NotifyTaskCompletionBase<TTask>> creator,
            bool disableMultipleRequests = true)
            : base(
                _ => { throw new InvalidOperationException("Should not be called"); }, // Dummy action sent to base. Not super nice.
                condition)
        {
            _action = action;
            _creator = creator;
            _disableMultipleRequests = disableMultipleRequests;
        }

        protected AsyncParameterCommandBase(
            Func<TParameter, TTask> action,
            Func<TTask, NotifyTaskCompletionBase<TTask>> creator,
            bool disableMultipleRequests = true)
            : this(
                action,
                _ => true,
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

        public async Task ExecuteAsync(TParameter parameter)
        {
            Execution = _creator(_action(parameter));
            RaiseCanExecuteChanged();
            await Execution.Completed;
        }

        protected override bool InternalCanExecute(TParameter parameter)
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

        protected override async void InternalExecute(TParameter parameter)
        {
            await ExecuteAsync(parameter);
            RaiseCanExecuteChanged();
        }
    }
}