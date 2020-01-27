namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;

    using Gu.Reactive;

    /// <summary>
    /// Runs tasks and notifies about status changes.
    /// </summary>
    /// <typeparam name="TParameter">The type of the command parameter.</typeparam>
    public interface ITaskRunner<in TParameter> : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// The status of the current task.
        /// </summary>
        NotifyTaskCompletion? TaskCompletion { get; }

        /// <summary>
        /// Condition for if the current run can be canceled.
        /// </summary>
        ICondition CanCancelCondition { get; }

        /// <summary>
        /// Condition for if the task can be executed.
        /// </summary>
        ICondition CanRunCondition { get; }

        /// <summary>
        /// Execute the task.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        void Run(TParameter parameter);

        /// <summary>
        /// Cancel the execution.
        /// </summary>
        void Cancel();
    }
}
