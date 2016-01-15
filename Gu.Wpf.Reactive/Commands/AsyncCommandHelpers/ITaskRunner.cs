namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;

    using Gu.Reactive;

    public interface ITaskRunner : INotifyPropertyChanged, IDisposable
    {
        NotifyTaskCompletion TaskCompletion { get; }

        ICondition CanCancelCondition { get; }

        ICondition CanRunCondition { get; }

        void Run();

        void Cancel();
    }
}