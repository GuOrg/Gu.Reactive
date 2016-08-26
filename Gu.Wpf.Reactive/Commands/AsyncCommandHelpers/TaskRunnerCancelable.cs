namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gu.Reactive;
    using Gu.Reactive.Internals;

    public class TaskRunnerCancelable : TaskRunnerBase, ITaskRunner
    {
        private readonly Func<CancellationToken, Task> action;
        private readonly SerialDisposable cancellationSubscription = new SerialDisposable();

        private CancellationTokenSource cancellationTokenSource;

        public TaskRunnerCancelable(Func<CancellationToken, Task> action)
        {
            Ensure.NotNull(action, nameof(action));
            this.action = action;

            var observable = Observable.Merge<object>(this.ObservePropertyChangedSlim(nameof(this.CanCancel)),
                                                      this.CanRunCondition.ObserveIsSatisfiedChanged());
            this.CanCancelCondition = new Condition(observable, () => this.CanCancel) { Name = "CanCancel" };
        }

        public void Run()
        {
            this.cancellationTokenSource?.Dispose();
            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationSubscription.Disposable = this.cancellationTokenSource.Token.AsObservable()
                                                                           .Subscribe(_ => this.OnPropertyChanged(nameof(this.CanCancel)));
            this.TaskCompletion = new NotifyTaskCompletion(this.action(this.cancellationTokenSource.Token));
        }

        public bool? CanCancel
        {
            get
            {
                if (this.CanRun()== true)
                {
                    return false;
                }
                var cts = this.cancellationTokenSource;
                if (cts == null)
                {
                    return false;
                }
                return !cts.IsCancellationRequested;
            }
        }

        public void Cancel()
        {
            this.cancellationTokenSource?.Cancel();
        }

        public override ICondition CanCancelCondition { get; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.cancellationTokenSource.Dispose();
                this.cancellationSubscription.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}