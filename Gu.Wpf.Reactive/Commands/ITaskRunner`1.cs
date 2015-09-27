namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;

    using Gu.Reactive;

    public interface ITaskRunner<TParameter> : INotifyPropertyChanged, IDisposable
    {
        NotifyTaskCompletion TaskCompletion { get; }

        ICondition CanCancelCondition { get; }

        ICondition CanRunCondition { get; }

        void Run(TParameter parameter);

        void Cancel();
    }
}