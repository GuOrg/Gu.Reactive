namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;
    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// </summary>
    public class AsyncCommand : ManualRelayCommand, IAsyncCommand
    {
        private readonly Func<object, Task> _action;
        private readonly bool _disableMultipleRequests;
        private NotifyTaskCompletion _execution;
        public AsyncCommand(
            Func<object, Task> action,
            Predicate<object> condition,
            bool disableMultipleRequests = true,
            bool raiseCanExecuteOnDispatcher = true)
            : base(_ => { throw new InvalidOperationException("Should not be called"); },
                condition, raiseCanExecuteOnDispatcher)
        {
            _action = action;
            _disableMultipleRequests = disableMultipleRequests;
        }

        public AsyncCommand(Func<object, Task> action, bool disableMultipleRequests = true, bool raiseCanExecuteOnDispatcher = true)
            : this(action, _ => true, disableMultipleRequests, raiseCanExecuteOnDispatcher)
        {
        }

        public NotifyTaskCompletion Execution
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

        public Task ExecuteAsync(object parameter)
        {
            Execution = new NotifyTaskCompletion(_action(parameter));
            RaiseCanExecuteChanged();
            return Execution.Task;
        }

        public bool CanExecute(object parameter)
        {
            if (_disableMultipleRequests)
            {
                if (Execution == null || Execution.IsCompleted)
                {
                    return false;
                }
            }

            return base.CanExecute(parameter);
        }

        public override async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
            RaiseCanExecuteChanged();
        }
    }
}