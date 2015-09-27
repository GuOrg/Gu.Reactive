namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    using Gu.Reactive;

    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// An async command that uses command parameter
    /// Returns a Task
    /// </summary>
    public class AsyncCommand<TParameter> : ConditionRelayCommand<TParameter>
    {
        private readonly TaskRunner<TParameter> _runner;

        public AsyncCommand(Func<TParameter,Task> action, params ICondition[] conditions)
            : this(new TaskRunner<TParameter>(action), conditions)
        {
        }

        public AsyncCommand(Func<TParameter, Task> action)
            : this(new TaskRunner<TParameter>(action), (ICondition[])null)
        {
        }

        private AsyncCommand(TaskRunner<TParameter> runner, ICondition[] conditions)
            : this(runner, runner.CreateCondition(conditions))
        {

        }

        private AsyncCommand(TaskRunner<TParameter> runner, ICondition condition)
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
