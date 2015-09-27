namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    using Gu.Reactive;

    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// An async command that does not use commandparameter
    /// Returns a Task
    /// </summary>
    public class AsyncCommand : ConditionRelayCommand
    {
        private readonly TaskRunner _runner;

        public AsyncCommand(Func<Task> action, params ICondition[] conditions)
            : this(new TaskRunner(action), conditions)
        {
        }

        public AsyncCommand(Func<Task> action)
            : this(new TaskRunner(action), (ICondition[])null)
        {
        }

        private AsyncCommand(TaskRunner runner, ICondition[] conditions)
            : this(runner, runner.CreateCondition(conditions))
        {

        }

        private AsyncCommand(TaskRunner runner, ICondition condition)
            : base(runner.Run, condition)
        {
            Condition = condition;
            _runner = runner;
            _runner.ObservePropertyChangedSlim(nameof(runner.TaskCompletion))
                   .Subscribe(_ => OnPropertyChanged(nameof(Execution)));
        }

        public ICondition Condition { get; }

        public NotifyTaskCompletion Execution => _runner.TaskCompletion;
    }
}