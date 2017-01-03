namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;

    using Gu.Reactive;

    /// <summary>
    /// Runs tasks and notifies about status changes.
    /// </summary>
    public interface ITaskRunner : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// The status of the current task.
        /// </summary>
        NotifyTaskCompletion TaskCompletion { get; }

        /// <summary>
        /// Condition for if the current run can be canceled.
        /// </summary>
        ICondition CanCancelCondition { get; }

        /// <summary>
        /// Condition for if the task can be executed.
        /// </summary>
        ICondition CanRunCondition { get; }

        /// <summary>
        /// Excecute the task.
        /// </summary>
        void Run();

        /// <summary>
        /// Cancel the execution.
        /// </summary>
        void Cancel();
    }
}