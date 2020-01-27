namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gu.Reactive;
    using Gu.Reactive.Internals;

    /// <summary>
    /// A task runner for generic tasks.
    /// </summary>
    /// <typeparam name="TParameter">The type of the command parameter.</typeparam>
    public class TaskRunnerCancelable<TParameter> : TaskRunnerBase, ITaskRunner<TParameter>
    {
        private readonly Func<TParameter, CancellationToken, Task> action;
        private readonly SerialDisposable cancellationSubscription = new SerialDisposable();

        private CancellationTokenSource? cancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskRunnerCancelable{TParameter}"/> class.
        /// </summary>
        /// <param name="action">The source of tasks to execute.</param>
        public TaskRunnerCancelable(Func<TParameter, CancellationToken, Task> action)
        {
            Ensure.NotNull(action, nameof(action));
            this.action = action;

            this.CanCancelCondition = new Condition(
                Observable.Merge<object>(
                    this.ObservePropertyChangedSlim(nameof(this.CanCancel)),
                    this.CanRunCondition.ObserveIsSatisfiedChanged())
                , () => this.CanCancel)
            {
                Name = "CanCancel"
            };
        }

        /// <inheritdoc/>
        public override ICondition CanCancelCondition { get; }

        /// <summary>
        /// Check if execution can be canceled.
        /// True if a cancellation token was provided and a task is running.
        /// </summary>
        public bool? CanCancel
        {
            get
            {
                if (this.CanRun() == true)
                {
                    return false;
                }

                var cts = this.cancellationTokenSource;
                if (cts is null)
                {
                    return false;
                }

                return !cts.IsCancellationRequested;
            }
        }

        /// <inheritdoc/>
        public void Run(TParameter parameter)
        {
            this.ThrowIfDisposed();
            this.cancellationTokenSource?.Dispose();
            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationSubscription.Disposable = this.cancellationTokenSource.Token.AsObservable()
                                                           .Subscribe(_ => this.OnPropertyChanged(nameof(this.CanCancel)));
            this.TaskCompletion = new NotifyTaskCompletion(this.action(parameter, this.cancellationTokenSource.Token));
        }

        /// <inheritdoc/>
        public override void Cancel()
        {
            this.ThrowIfDisposed();
            this.cancellationTokenSource?.Cancel();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.cancellationTokenSource?.Dispose();
                this.cancellationSubscription.Dispose();
                this.CanCancelCondition.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
