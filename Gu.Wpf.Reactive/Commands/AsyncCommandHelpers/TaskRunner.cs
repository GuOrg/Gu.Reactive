namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    using Gu.Reactive.Internals;

    public class TaskRunner : TaskRunnerBase, ITaskRunner
    {
        private readonly Func<Task> action;

        public TaskRunner(Func<Task> action)
        {
            Ensure.NotNull(action, nameof(action));
            this.action = action;
        }

        public void Run()
        {
            this.TaskCompletion = new NotifyTaskCompletion(this.action());
        }
    }
}