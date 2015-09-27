namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    using Gu.Reactive.Internals;

    public class TaskRunner : TaskRunnerBase
    {
        private readonly Func<Task> _action;

        public TaskRunner(Func<Task> action)
        {
            Ensure.NotNull(action, nameof(action));
            _action = action;
        }

        public void Run()
        {
            TaskCompletion = new NotifyTaskCompletion(_action());
        }
    }
}