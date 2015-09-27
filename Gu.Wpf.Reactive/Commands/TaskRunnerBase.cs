namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Gu.Reactive;
    using Gu.Wpf.Reactive.Annotations;

    public abstract class TaskRunnerBase : INotifyPropertyChanged
    {
        private NotifyTaskCompletion _taskCompletion;

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public NotifyTaskCompletion TaskCompletion
        {
            get { return _taskCompletion; }
            protected set
            {
                if (Equals(value, _taskCompletion))
                {
                    return;
                }
                _taskCompletion = value;
                OnPropertyChanged();
            }
        }

        public bool? CanRun()
        {
            var completion = TaskCompletion;
            if (completion == null)
            {
                return true;
            }
            switch (completion.Task.Status)
            {
                case TaskStatus.Created:
                case TaskStatus.WaitingForActivation:
                case TaskStatus.WaitingToRun:
                case TaskStatus.Running:
                case TaskStatus.WaitingForChildrenToComplete:
                    return false;
                case TaskStatus.RanToCompletion:
                case TaskStatus.Canceled:
                case TaskStatus.Faulted:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ICondition CreateCondition(ICondition[] conditions)
        {
            var observable = this.ObservePropertyChanged(x => x.TaskCompletion.Status);

            var canRun = new Condition(observable, CanRun) { Name = "CanRun" };

            if (conditions == null || conditions.Length == 0)
            {
                return canRun;
            }

            return new AndCondition(canRun, conditions);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}