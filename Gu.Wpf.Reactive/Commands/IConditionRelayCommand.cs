namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows.Input;

    using Gu.Reactive;

    public interface IConditionRelayCommand : ICommand, IDisposable
    {
        ICondition Condition { get; }
    }
}