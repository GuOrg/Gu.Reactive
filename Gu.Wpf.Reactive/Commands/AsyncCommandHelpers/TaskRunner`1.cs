namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    using Gu.Reactive.Internals;

    public class TaskRunner<TParameter> : TaskRunnerBase, ITaskRunner<TParameter>
    {
        private readonly Func<TParameter,Task> action;

        public TaskRunner(Func<TParameter,Task> action)
        {
            Ensure.NotNull(action, nameof(action));
            this.action = action;
        }

        public void Run(TParameter parameter)
        {
            this.TaskCompletion = new NotifyTaskCompletion(this.action(parameter));
        }
    }
}
