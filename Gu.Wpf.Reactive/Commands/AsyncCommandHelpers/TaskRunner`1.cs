namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    using Gu.Reactive.Internals;

    public class TaskRunner<TParameter> : TaskRunnerBase, ITaskRunner<TParameter>
    {
        private readonly Func<TParameter,Task> _action;

        public TaskRunner(Func<TParameter,Task> action)
        {
            Ensure.NotNull(action, nameof(action));
            _action = action;
        }

        public void Run(TParameter parameter)
        {
            TaskCompletion = new NotifyTaskCompletion(_action(parameter));
        }
    }
}
