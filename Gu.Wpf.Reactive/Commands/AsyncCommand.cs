namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Gu.Reactive;

    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// An async command that does not use commandparameter
    /// Returns a Task
    /// </summary>
    public class AsyncCommand : ConditionRelayCommand
    {
        private readonly ITaskRunner _runner;

        public AsyncCommand(Func<Task> action, params ICondition[] conditions)
            : this(new TaskRunner(action), conditions)
        {
        }

        public AsyncCommand(Func<Task> action)
            : this(new TaskRunner(action))
        {
        }

        public AsyncCommand(Func<CancellationToken,Task> action, params ICondition[] conditions)
            : this(new TaskRunnerCancelable(action), conditions)
        {
        }

        public AsyncCommand(Func<CancellationToken, Task> action)
            : this(new TaskRunnerCancelable(action))
        {
        }

        private AsyncCommand(ITaskRunner runner, ICondition[] conditions)
            : this(runner, new AndCondition(runner.CanRunCondition, conditions))
        {
        }

        private AsyncCommand(ITaskRunner runner)
            : this(runner, runner.CanRunCondition)
        {

        }
        private AsyncCommand(ITaskRunner runner, ICondition condition)
            : base(runner.Run, condition)
        {
            CancelCommand = new ConditionRelayCommand(runner.Cancel, runner.CanCancelCondition);
            _runner = runner;
            _runner.ObservePropertyChangedSlim(nameof(runner.TaskCompletion))
                   .Subscribe(_ => OnPropertyChanged(nameof(Execution)));
        }

        public ConditionRelayCommand CancelCommand { get; }

        public NotifyTaskCompletion Execution => _runner.TaskCompletion;
    }
}